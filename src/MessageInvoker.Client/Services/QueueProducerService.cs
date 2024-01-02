using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messageing.ServiceBus.Invoker.Client.Helpers;
using Azure.Messageing.ServiceBus.Invoker.Client.Messages;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Amqp.Serialization;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Azure.Messageing.ServiceBus.Invoker.Client.Services
{

    internal class QueueProducerService : IQueueProducerService
    {       
        private readonly ServiceBusSender _serviceBusSender;        

        public QueueProducerService(ServiceBusClient serviceBusClient, string queueName)
        {     
            _serviceBusSender = serviceBusClient.CreateSender(queueName);
        }

        public async Task<ServiceBusMessage> SubmitMethodStringToQueue(string fullyQualifiedTypeName, string methodName, object[] parameters, string tag = null)
        {
            var mc = new InvocationMessage(fullyQualifiedTypeName, methodName, parameters, tag);

            return await SendMessage(mc);
        }

        public async Task<ServiceBusMessage> SubmitMethodExpressionToQueue<TClass>(Expression<Action<TClass>> expression, string tag = null)
        {
            var parameters = ExpressionHelpers.GetParameters(expression);
            var methodName = ExpressionHelpers.GetMethodName(expression);
            var callers = ExpressionHelpers.GetCallers(expression);

            var mc = new InvocationMessage<TClass>(parameters, methodName, callers, tag);

            return await SendMessage(mc);
        }

        public virtual async Task<ServiceBusMessage> SendMessage(IInvocationMessage mc)
        {
            var byteStream = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mc));

            var message = new ServiceBusMessage(byteStream)
            {
                MessageId = GenerateId(mc),
                ContentType = "application/json"
            };

            await _serviceBusSender.SendMessageAsync(message);

            return message;
        }

        private string GenerateId(IInvocationMessage mc)
        {
            var jObject = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mc));

            var encryption = new Hasher();

            return encryption.ComputeHash(jObject);
        }
    }
}
