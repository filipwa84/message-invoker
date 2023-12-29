using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messageing.ServiceBus.Invoker.MessageContainers;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Azure.Messageing.ServiceBus.Invoker.Services
{
    internal class QueueConsumerService : IQueueConsumerService
    {        
        private readonly string queueName;
        private ServiceBusClient _serviceBusClient;
        private ServiceBusReceiver _serviceBusReceiver;
        
        public QueueConsumerService(ServiceBusClient serviceBusClient, string queueName)
        {   
            this.queueName = queueName;
            _serviceBusClient = serviceBusClient;
            _serviceBusReceiver = serviceBusClient.CreateReceiver(queueName);
        }

        public async Task ClearQueueAsync()
        {
            var loop = true;

            while(loop)
            {
                var messages = (await _serviceBusReceiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5)));
                
                if (messages == null) break;
               
                loop = messages.Any();
            }
        }

        public void ClearQueue(bool verbose = false)
        {
            ClearQueueAsync().GetAwaiter().GetResult();
            if(verbose)
                Console.WriteLine("Queue was successfully cleared");
        }

        public async Task<ServiceBusReceivedMessage> GetMessageAsync()
        { 
            return await _serviceBusReceiver.ReceiveMessageAsync(); 
        }

        private void ProcessMethodInvocation(object container, ServiceBusReceivedMessage message)
        {
            var json = Encoding.UTF8.GetString(message.Body);
            var mc = JsonConvert.DeserializeObject<InvocationMessage>(json);

            var startTime = DateTime.UtcNow;
            
            var tag = string.IsNullOrEmpty(mc.Tag) ? string.Empty : $"Tag: {mc.Tag}. ";

            Console.WriteLine($"Invoking: {mc.MethodName}. MessageId: {message.MessageId}. {tag}Started at: {startTime}.");
            try
            {
                var messageInvoker = new MessageInvokerService();
                messageInvoker.Invoke(container, mc);
                var endTime = DateTime.UtcNow;
                Console.WriteLine($"Invocation completed: {mc.MethodName}. MessageId: {message.MessageId}. {tag}Completed at: {endTime}. Duration: {Convert.ToInt32((endTime - startTime).TotalMilliseconds)}ms");
            }
            catch(Exception e)
            {
                Console.WriteLine($"Invocation failed: {mc.MethodName}. MessageId: {message.MessageId}. {tag}Failed at: {DateTime.UtcNow}");
                throw e;
            }
        }

        private async Task ReceiveMessageAndProcessMethodInvocation(object container)
        {
            var message = await _serviceBusReceiver.ReceiveMessageAsync();

            ProcessMethodInvocation(container, message);
        }       
        
        public Task ProcessMethodInvocation(IServiceProvider serviceProvider)
        {
            return ReceiveMessageAndProcessMethodInvocation(container: serviceProvider);
        }

        public void ProcessMethodInvocation(IServiceProvider serviceProvider, ServiceBusReceivedMessage message)
        {
            ProcessMethodInvocation(container: serviceProvider, message);
        }
    }   
}
