using System;
using System.Collections.Concurrent;
using Azure.Messageing.ServiceBus.Invoker.Client.Services;
using Azure.Messaging.ServiceBus;

namespace Azure.Messageing.ServiceBus.Invoker.Client
{
    public class MessageInvocationClient : IMessageInvocationClient
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IServiceProvider _serviceProvider;

        public IQueueConsumerService QueueConsumerByQueueName(string queueName) => GetConsumer(queueName);
        public IQueueProducerService QueueProducerByQueueName(string queueName) => GetProducer(queueName);

        private readonly ConcurrentDictionary<string, IQueueConsumerService> _consumers = new ConcurrentDictionary<string, IQueueConsumerService>();
        private readonly ConcurrentDictionary<string, IQueueProducerService> _producers = new ConcurrentDictionary<string, IQueueProducerService>();

        public MessageInvocationClient(string connectionString, IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception($"The {nameof(MessageInvocationClient)} requires a connection string in order to be constructed.");

            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            var options = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            _serviceBusClient = new ServiceBusClient(connectionString, options);
            _serviceProvider = serviceProvider;
        }

        private IQueueConsumerService GetConsumer(string queueName)
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

        private IQueueProducerService GetProducer(string queueName)
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
