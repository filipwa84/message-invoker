using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace Azure.Messaging.ServiceBus.Invoker.Services
{
    public interface IQueueConsumerService
    {
        Task<ServiceBusReceivedMessage> GetMessageAsync();
        Task ProcessMethodInvocation();
        void ProcessMethodInvocation(ServiceBusReceivedMessage message);
    }
}