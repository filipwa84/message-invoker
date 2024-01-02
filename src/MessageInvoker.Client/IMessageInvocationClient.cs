using Azure.Messageing.ServiceBus.Invoker.Client.Services;

namespace Azure.Messageing.ServiceBus.Invoker.Client
{
    public interface IMessageInvocationClient
    {
        IQueueConsumerService QueueConsumerByQueueName(string queueName);
        IQueueProducerService QueueProducerByQueueName(string queueName);
    }
}
