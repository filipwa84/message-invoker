using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Azure.Messageing.ServiceBus.Invoker.Client.MethodTransporters;
using Azure.Messageing.ServiceBus.Invoker.Client.Services;
using Azure.Messaging.ServiceBus;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Azure.Messageing.ServiceBus.Invoker.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Azure.Messageing.ServiceBus.Invoker.Client.Helpers;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Azure.Messageing.ServiceBus.Invoker.Tests
{
    [TestFixture]
    public class QueueProducerConsumerTests
    {
        private IFakeService _fakeService;
        private IServiceProvider _serviceProvider;
        private IQueueProducerService _queueProducerService;
        private IQueueConsumerService _queueConsumerService;

        private Queue<ServiceBusMessage> _messages = new Queue<ServiceBusMessage>();

        [SetUp]
        public void Setup()
        {
            _fakeService = new FakeService();

            var mockServiceContainer = new Mock<IServiceProvider>();
            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();
            var mockServiceBusSender = new Mock<ServiceBusSender>();
            var mockServiceBusReceiver = new Mock<ServiceBusReceiver>(MockBehavior.Loose);
            var mockServiceBusClient = new Mock<ServiceBusClient>(MockBehavior.Loose);
            var mockQueueProducerService = new Mock<QueueProducerService>(mockServiceBusClient.Object, "fake-queue-name");

            mockServiceContainer.Setup(x => x.GetService(typeof(IFakeService))).Returns(_fakeService);
            mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
            mockScope.Setup(x => x.ServiceProvider).Returns(mockServiceContainer.Object);
            mockServiceContainer.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);
            mockServiceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns<string>(x => mockServiceBusSender.Object);
            mockServiceBusClient.Setup(x => x.CreateReceiver(It.IsAny<string>())).Returns<string>(x => mockServiceBusReceiver.Object);
            mockServiceBusReceiver.Setup(x => x.ReceiveMessageAsync(It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>())).Returns(() =>
            {
                var message = GetServiceBusMessage();

                var amqpMessage = message.GetRawAmqpMessage();

                var receivedMessage = ServiceBusReceivedMessage.FromAmqpMessage(amqpMessage, BinaryData.FromBytes(message.Body));

                return Task.FromResult(receivedMessage);
            });
            mockQueueProducerService.Setup(x => x.SendMessage(It.IsAny<IInvocationMessage>()))
                .Returns<IInvocationMessage>(message =>
                {
                    var serviceBusMessage = GetServiceBusMessage(message);

                    return Task.FromResult(serviceBusMessage);

                });

            _serviceProvider = mockServiceContainer.Object;
            _queueConsumerService = new QueueConsumerService(_serviceProvider, mockServiceBusClient.Object, "fake-queue-name");
            _queueProducerService = mockQueueProducerService.Object;
        }

        private ServiceBusMessage GetServiceBusMessage(IInvocationMessage invocationMessage = null)
        {
            if (invocationMessage == null)
            {
                return _messages.Dequeue();
            }

            var byteStream = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(invocationMessage));

            var serviceBusMessage = new ServiceBusMessage(byteStream)
            {
                ContentType = "application/json",
                MessageId = "someid"
            };

            _messages.Enqueue(serviceBusMessage);

            return serviceBusMessage;
        }

        [Test]
        public async Task SubmitMethodExpressionToQueueTest()
        {
            var serviceBusMessage = await _queueProducerService.SubmitMethodExpressionToQueue<IFakeService>(service => service.StringParameterMethod("Success"));
            
            Assert.DoesNotThrow(() => _queueConsumerService.ProcessMethodInvocation().GetAwaiter().GetResult());
        }

        [Test]
        public async Task SubmitMethodStringToQueueTest()
        {
            var serviceBusMessage = await _queueProducerService.SubmitMethodStringToQueue("Azure.Messageing.ServiceBus.Invoker.Tests.Services.IFakeService", "StringParameterMethod", new object[] { "Success" });

            Assert.DoesNotThrow(() => _queueConsumerService.ProcessMethodInvocation().GetAwaiter().GetResult());
        }
    }
}
