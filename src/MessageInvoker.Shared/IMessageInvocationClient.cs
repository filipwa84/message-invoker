using MessageInvoker.Shared.Services;

namespace MessageInvoker.Shared
{
    public interface IMessageInvocationClient<TMessage, TReceiver>
    {
        IQueueConsumerService<TReceiver> QueueConsumerByQueueName(string queueName);
        IQueueProducerService<TMessage> QueueProducerByQueueName(string queueName);
    }
}
