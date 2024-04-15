using Azure.Messaging.ServiceBus.Invoker.Services;

namespace Azure.Messaging.ServiceBus.Invoker
{
    public interface IMessageInvocationClient
    {
        IQueueConsumerService QueueConsumerByQueueName(string queueName);
        IQueueProducerService QueueProducerByQueueName(string queueName);
    }
}
