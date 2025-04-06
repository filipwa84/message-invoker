using Azure.Messaging.ServiceBus;
using MessageInvoker.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageInvoker.AzureServiceBus.Services
{
    public interface IInvocationClient : IMessageInvocationClient<ServiceBusMessage, ServiceBusReceivedMessage>
    {
    }
}
