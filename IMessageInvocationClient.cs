using Azure.Messageing.ServiceBus.Invoker.Services;

namespace Azure.Messageing.ServiceBus.Invoker
{
    public interface IMessageInvocationClient
    {        
        IQueueConsumerService QueueConsumerByQueueName(string queueName);
        IQueueProducerService QueueProducerByQueueName(string queueName);
    }
}
