using MessageInvoker.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageInvoker.RabbitMQ.Services
{
    public interface IRabbitMqMessageClient : IMessageInvocationClient<byte[], byte[]>
    {
    }
}
