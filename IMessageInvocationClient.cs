﻿using Azure.Messageing.ServiceBus.Invoker.src.MessageInvoker.Client.Services;

namespace Azure.Messageing.ServiceBus.Invoker.src.MessageInvoker.Client
{
    public interface IMessageInvocationClient
    {
        IQueueConsumerService QueueConsumerByQueueName(string queueName);
        IQueueProducerService QueueProducerByQueueName(string queueName);
    }
}
