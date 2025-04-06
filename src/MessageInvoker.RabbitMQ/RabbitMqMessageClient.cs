using MessageInvoker.RabbitMQ.Services;
using MessageInvoker.Shared;
using MessageInvoker.Shared.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;

namespace MessageInvoker.RabbitMQ
{
    public class RabbitMqMessageClient : IRabbitMqMessageClient
    {
        private readonly IChannelFactory _channelFactory;
        private readonly IServiceProvider _serviceProvider;

        private readonly ConcurrentDictionary<string, IQueueConsumerService<byte[]>> _consumers = new ConcurrentDictionary<string, IQueueConsumerService<byte[]>>();

        private readonly ConcurrentDictionary<string, IQueueProducerService<byte[]>> _producers = new ConcurrentDictionary<string, IQueueProducerService<byte[]>>();

        public RabbitMqMessageClient(IServiceProvider serviceProvider, IChannelFactory channelFactory)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _serviceProvider = serviceProvider;
            _channelFactory = channelFactory;
        }

        public IQueueConsumerService<byte[]> QueueConsumerByQueueName(string queueName) => GetConsumer(queueName);

        public IQueueProducerService<byte[]> QueueProducerByQueueName(string queueName) => GetProducer(queueName);

        private IQueueConsumerService<byte[]> GetConsumer(string queueName)
        {
            if (string.IsNullOrEmpty(queueName) || _channelFactory == null)
                throw new Exception("Invalid configuration");

            if (_consumers.ContainsKey(queueName))
            {
                return _consumers[queueName];
            }

            var consumer = new QueueConsumerService(_serviceProvider, _channelFactory, queueName);
            _consumers.TryAdd(queueName, consumer);

            return consumer;
        }

        private IQueueProducerService<byte[]> GetProducer(string queueName)
        {
            if (string.IsNullOrEmpty(queueName) || _channelFactory == null)
                throw new Exception("Invalid configuration");

            if (_producers.ContainsKey(queueName))
            {
                return _producers[queueName];
            }

            var producer = new QueueProducerService(_channelFactory, queueName);
            _producers.TryAdd(queueName, producer);

            return producer;
        }
    }
}
