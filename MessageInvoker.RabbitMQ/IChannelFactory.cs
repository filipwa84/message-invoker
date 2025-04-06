using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageInvoker.RabbitMQ
{
    public interface IChannelFactory
    {
        Task<IChannel> GetChannelAsync();
    }
}
