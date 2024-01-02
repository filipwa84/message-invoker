using Azure.Messageing.ServiceBus.Invoker.Client.MethodTransporters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Azure.Messageing.ServiceBus.Invoker.Client.Services
{
    public interface IMessageInvoker
    {
        object Invoke(InvocationMessage message);
        object GetMemberValue(MemberExpression exp);
    }
}