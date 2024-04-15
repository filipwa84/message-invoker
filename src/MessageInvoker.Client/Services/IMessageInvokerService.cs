using Azure.Messaging.ServiceBus.Invoker.Messages;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Azure.Messaging.ServiceBus.Invoker.Services
{
    public interface IMessageInvoker
    {
        object Invoke(InvocationMessage message);
        object GetMemberValue(MemberExpression exp);
    }
}