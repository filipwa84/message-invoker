using System;
using System.Collections.Concurrent;
using Azure.Messaging.ServiceBus;
using MessageInvoker.Shared;
using MessageInvoker.Shared.Services;
using MessageInvoker.AzureServiceBus.Services;

namespace MessageInvoker.AzureServiceBus
{
    public class InvocationClient : IInvocationClient
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IServiceProvider _serviceProvider;

        public IQueueConsumerService<ServiceBusReceivedMessage> QueueConsumerByQueueName(string queueName) => GetConsumer(queueName);
        public IQueueProducerService<ServiceBusMessage> QueueProducerByQueueName(string queueName) => GetProducer(queueName);

        private readonly ConcurrentDictionary<string, IQueueConsumerService<ServiceBusReceivedMessage>> _consumers = new ConcurrentDictionary<string, IQueueConsumerService<ServiceBusReceivedMessage>>();
        private readonly ConcurrentDictionary<string, IQueueProducerService<ServiceBusMessage>> _producers = new ConcurrentDictionary<string, IQueueProducerService<ServiceBusMessage>>();

        public InvocationClient(IServiceProvider serviceProvider, string connectionString, ServiceBusTransportType transportType)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception($"The {nameof(InvocationClient)} requires a connection string in order to be constructed.");

            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var options = new ServiceBusClientOptions
            {
                TransportType = transportType
            };

            _serviceBusClient = new ServiceBusClient(connectionString, options);
            _serviceProvider = serviceProvider;
        }

        private IQueueConsumerService<ServiceBusReceivedMessage> GetConsumer(string queueName)
        {
            if (string.IsNullOrEmpty(queueName) || _serviceBusClient == null)
                throw new Exception("Invalid configuration");

            if (_consumers.ContainsKey(queueName))
            {
                return _consumers[queueName];
            }

            var consumer = new QueueConsumerService(_serviceProvider, _serviceBusClient, queueName);

            _consumers.TryAdd(queueName, consumer);

            return consumer;
        }

        private IQueueProducerService<ServiceBusMessage> GetProducer(string queueName)
        {
            if (string.IsNullOrEmpty(queueName) || _serviceBusClient == null)
                throw new Exception("Invalid configuration");

            if (_producers.ContainsKey(queueName))
            {
                return _producers[queueName];
            }

            var producer = new QueueProducerService(_serviceBusClient, queueName);

            _producers.TryAdd(queueName, producer);

            return producer;
        }
    }
}
