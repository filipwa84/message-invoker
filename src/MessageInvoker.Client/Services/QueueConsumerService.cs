using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Invoker.Messages;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Azure.Messaging.ServiceBus.Invoker.Services
{
    internal class QueueConsumerService : IQueueConsumerService
    {
        private readonly ServiceBusReceiver _serviceBusReceiver;
        private readonly IServiceProvider _serviceProvider;

        public QueueConsumerService(IServiceProvider serviceProvider, ServiceBusClient serviceBusClient, string queueName)
        {
            _serviceBusReceiver = serviceBusClient.CreateReceiver(queueName);
            _serviceProvider = serviceProvider;
        }
        
        public async Task<ServiceBusReceivedMessage> GetMessageAsync()
        {
            return await _serviceBusReceiver.ReceiveMessageAsync();
        }

        public Task ProcessMethodInvocation()
        {
            return ReceiveMessageAndProcessMethodInvocation();
        }

        public void ProcessMethodInvocation(ServiceBusReceivedMessage message)
        {
            var json = Encoding.UTF8.GetString(message.Body);
            var mc = JsonConvert.DeserializeObject<InvocationMessage>(json);

            var startTime = DateTime.UtcNow;

            var tag = string.IsNullOrEmpty(mc.Tag) ? string.Empty : $"Tag: {mc.Tag}. ";

            Console.WriteLine($"Invoking: {mc.MethodName}. MessageId: {message.MessageId}. {tag}Started at: {startTime}.");
            try
            {
                var messageInvoker = new MessageInvoker(_serviceProvider);
                messageInvoker.Invoke(mc);
                var endTime = DateTime.UtcNow;
                Console.WriteLine($"Invocation completed: {mc.MethodName}. MessageId: {message.MessageId}. {tag}Completed at: {endTime}. Duration: {Convert.ToInt32((endTime - startTime).TotalMilliseconds)}ms");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Invocation failed: {mc.MethodName}. MessageId: {message.MessageId}. {tag}Failed at: {DateTime.UtcNow}");
                throw e;
            }
        }

        private async Task ReceiveMessageAndProcessMethodInvocation()
        {
            var message = await _serviceBusReceiver.ReceiveMessageAsync();

            ProcessMethodInvocation(message);
        }
    }
}
