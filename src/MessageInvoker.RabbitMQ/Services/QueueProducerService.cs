using MessageInvoker.Shared.Helpers;
using MessageInvoker.Shared.Messages;
using MessageInvoker.Shared.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MessageInvoker.RabbitMQ.Services
{
    public class QueueProducerService : IQueueProducerService<byte[]>
    {
        private readonly IChannelFactory _channelFactory;
        private readonly string _queueName;

        public QueueProducerService(IChannelFactory channelFactory, string queueName)
        {
            _channelFactory = channelFactory;
            _queueName = queueName;
        }

        public async Task<byte[]> SubmitMethodExpressionToQueue<TService>(System.Linq.Expressions.Expression<Action<TService>> expression, string tag = null)
        {
            var parameters = ExpressionHelpers.GetParameters(expression);
            var methodName = ExpressionHelpers.GetMethodName(expression);
            var callers = ExpressionHelpers.GetCallers(expression);

            var mc = new InvocationMessage<TService>(parameters, methodName, callers, tag);

            return await SendMessage(mc);
        }

        public async Task<byte[]> SubmitMethodStringToQueue(string fullyQualifiedTypeName, string methodName, object[] parameters, string tag = null)
        {
            var mc = new InvocationMessage(fullyQualifiedTypeName, methodName, parameters, tag);

            return await SendMessage(mc);
        }

        public async Task<byte[]> SendMessage(IInvocationMessage message)
        {
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var channel = await _channelFactory.GetChannelAsync();

            await channel.BasicPublishAsync("", _queueName, body);

            return body;
        }
    }
}