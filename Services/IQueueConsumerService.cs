using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs.ServiceBus;

namespace Azure.Messageing.ServiceBus.Invoker.Services
{
    public interface IQueueConsumerService
    {
        Task<ServiceBusReceivedMessage> GetMessageAsync();        
        Task ProcessMethodInvocation(IServiceProvider serviceProvider);
        void ProcessMethodInvocation(IServiceProvider serviceProvider, ServiceBusReceivedMessage message);
        Task ClearQueueAsync();
        void ClearQueue(bool verbose = false);        
    }
}