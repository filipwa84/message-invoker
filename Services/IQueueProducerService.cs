using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Azure.Messageing.ServiceBus.Invoker.Services
{
    public interface IQueueProducerService
    {
        /// <summary>
        /// Submits a message for remote execution to an AzureServiceBus queue. This call should only be used when the message producer does not reference the service directly but the consumer does. 
        /// </summary>
        /// <param name="fullyQualifiedTypeName">Fully qualified type name of service containing the method to be executed by the remote consumer.</param>
        /// <param name="methodName">The name of the method to be executed.</param>
        /// <param name="parameters">Array of method parameters.</param>
        /// <param name="tag">Message tag that can be used to identify the execution.</param>
        /// <returns>Service bus message</returns>
        Task<ServiceBusMessage> SubmitMethodStringToQueue(string fullyQualifiedTypeName, string methodName, object[] parameters, string tag = null);

        /// <summary>
        /// Submits a message for remote execution to an AzureServiceBus queue.
        /// </summary>
        /// <typeparam name="TService">Service containing the method to be executed by the remote consumer.</typeparam>
        /// <param name="expression">Expression of method call to be executed.</param>
        /// <param name="tag">Message tag that can be used to identify the execution.</param>
        /// <returns>Service bus message</returns>
        Task<ServiceBusMessage> SubmitMethodExpressionToQueue<TService>(Expression<Action<TService>> expression, string tag = null);          
    }
}