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

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Azure.Messageing.ServiceBus.Invoker.Tests
{
    [TestFixture]
    public class QueueProducerServiceTests
    {
        private IFakeService _fakeService;
        private IServiceProvider _serviceProvider;
        private IQueueProducerService _queueProducerService;

        [SetUp]
        public void Setup()
        {

            _fakeService = new FakeService();

            var mockServiceContainer = new Mock<IServiceProvider>();
            var mockScope = new Mock<IServiceScope>();
            var mockScopeFactory = new Mock<IServiceScopeFactory>();

            mockServiceContainer.Setup(x => x.GetService(typeof(IFakeService))).Returns(_fakeService);
            mockScopeFactory.Setup(x => x.CreateScope()).Returns(mockScope.Object);
            mockScope.Setup(x => x.ServiceProvider).Returns(mockServiceContainer.Object);
            mockServiceContainer.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(mockScopeFactory.Object);

            _serviceProvider = mockServiceContainer.Object;

            var mockServiceBusSender = new Mock<ServiceBusSender>();

            var mockServiceBusClient = new Mock<ServiceBusClient>(MockBehavior.Loose);
            mockServiceBusClient.Setup(x => x.CreateSender(It.IsAny<string>())).Returns<string>(x => mockServiceBusSender.Object);

            var mockQueueProducerService = new Mock<QueueProducerService>(mockServiceBusClient.Object, "fake-queue-name");
            mockQueueProducerService.Setup(x => x.SendMessage(It.IsAny<IInvocationMessage>()))
                .Returns<IInvocationMessage>(message =>
                {
                    var byteStream = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                    var serviceBusMessage = new ServiceBusMessage(byteStream)
                    {   
                        ContentType = "application/json"                        
                    };

                    return Task.FromResult(serviceBusMessage);

                });

            _queueProducerService = mockQueueProducerService.Object;

        }

        [Test]
        public async Task SubmitMethodExpressionToQueueTest()
        {
            var serviceBusMessage = await _queueProducerService.SubmitMethodExpressionToQueue<IFakeService>(service => service.StringParameterMethod("Success"));
            var json = Encoding.UTF8.GetString(serviceBusMessage.Body);
            var invocationMessage = JsonConvert.DeserializeObject<InvocationMessage>(json);

            var messageInvoker = new MessageInvoker(_serviceProvider);

            Assert.DoesNotThrow(() => messageInvoker.Invoke(invocationMessage));
        }

        [Test]
        public async Task SubmitMethodStringToQueueTest()
        {
            var serviceBusMessage = await _queueProducerService.SubmitMethodStringToQueue("Azure.Messageing.ServiceBus.Invoker.Tests.Services.IFakeService", "StringParameterMethod", new object[]{"Success"});
            var json = Encoding.UTF8.GetString(serviceBusMessage.Body);
            var invocationMessage = JsonConvert.DeserializeObject<InvocationMessage>(json);

            var messageInvoker = new MessageInvoker(_serviceProvider);

            Assert.DoesNotThrow(() => messageInvoker.Invoke(invocationMessage));
        }
    }
}
