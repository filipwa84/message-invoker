﻿using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Collections.Generic;
using MessageInvoker.Tests.Services;
using MessageInvoker.Shared.Services;
using MessageInvoker.AzureServiceBus.Services;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace MessageInvoker.Tests
{
    [TestFixture]
    public class AzureServiceBusQueueProducerConsumerTests
    {
        private IFakeService _fakeService;
        private IServiceProvider _serviceProvider;
        private IQueueProducerService<ServiceBusMessage> _queueProducerService;
        private IQueueConsumerService<ServiceBusReceivedMessage> _queueConsumerService;

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

            mockServiceBusSender.Setup(x => x.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>())).Returns<ServiceBusMessage, CancellationToken>((message, token) =>
            {
                GetServiceBusMessage(message);

                return Task.CompletedTask;

            });

            _serviceProvider = mockServiceContainer.Object;
            _queueConsumerService = new QueueConsumerService(_serviceProvider, mockServiceBusClient.Object, "fake-queue-name");
            _queueProducerService = new QueueProducerService(mockServiceBusClient.Object, "fake-queue-name");
        }

        private ServiceBusMessage GetServiceBusMessage(ServiceBusMessage serviceBusMessage = null)
        {
            if (serviceBusMessage == null)
            {
                return _messages.Dequeue();
            }
            _messages.Enqueue(serviceBusMessage);

            return serviceBusMessage;
        }

        [Test]
        public async Task SubmitMethodExpressionToQueueTest()
        {
            var serviceBusMessage = await _queueProducerService.SubmitMethodExpressionToQueue<IFakeService>(service => service.StringParameterMethod("Success"));

            NUnit.Framework.Assert.DoesNotThrow(() => _queueConsumerService.ProcessMethodInvocation().GetAwaiter().GetResult());
        }

        [Test]
        public async Task SubmitMethodStringToQueueTest()
        {
            var serviceBusMessage = await _queueProducerService.SubmitMethodStringToQueue("MessageInvoker.Tests.Services.IFakeService", "StringParameterMethod", new object[] { "Success" });

            NUnit.Framework.Assert.DoesNotThrow(() => _queueConsumerService.ProcessMethodInvocation().GetAwaiter().GetResult());
        }
    }
}
