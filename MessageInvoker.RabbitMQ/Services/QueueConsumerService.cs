using MessageInvoker.Shared.Messages;
using MessageInvoker.Shared.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MessageInvoker.RabbitMQ.Services
{
    public class QueueConsumerService : IQueueConsumerService<byte[]>
    {
        private IServiceProvider _serviceProvider;
        private readonly IChannelFactory _channelFactory;
        private readonly string _queueName;

        public QueueConsumerService(IServiceProvider serviceProvider, IChannelFactory channelFactory, string queueName)
        {
            _serviceProvider = serviceProvider;
            _channelFactory = channelFactory;
            _queueName = queueName;
        }

        public async Task<byte[]> GetMessageAsync()
        {

            var channel = await _channelFactory.GetChannelAsync();
            var message = await channel.BasicGetAsync(_queueName, true);

            return message?.Body.ToArray() ?? new byte[0];
        }

        public Task ProcessMethodInvocation()
        {
            throw new NotImplementedException();
        }

        public void ProcessMethodInvocation(byte[] message)
        {
            var json = Encoding.UTF8.GetString(message);
            var mc = JsonConvert.DeserializeObject<InvocationMessage>(json);

            var startTime = DateTime.UtcNow;

            var tag = string.IsNullOrEmpty(mc.Tag) ? string.Empty : $"Tag: {mc.Tag}. ";

            Console.WriteLine($"Invoking: {mc.MethodName}. {tag}Started at: {startTime}.");
            try
            {
                var messageInvoker = new MessageInvokerService(_serviceProvider);
                messageInvoker.Invoke(mc);
                var endTime = DateTime.UtcNow;
                Console.WriteLine($"Invocation completed: {mc.MethodName}. {tag}Completed at: {endTime}. Duration: {Convert.ToInt32((endTime - startTime).TotalMilliseconds)}ms");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Invocation failed: {mc.MethodName}. {tag}Failed at: {DateTime.UtcNow}");
                throw e;
            }
        }
    }
}