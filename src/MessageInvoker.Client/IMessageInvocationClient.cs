using Azure.Messaging.ServiceBus.Invoker.Client.Services;

namespace Azure.Messaging.ServiceBus.Invoker.Client
{
    public interface IMessageInvocationClient
    {
        IQueueConsumerService QueueConsumerByQueueName(string queueName);
        IQueueProducerService QueueProducerByQueueName(string queueName);
    }
}
