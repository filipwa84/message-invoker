using Azure.Messageing.ServiceBus.Invoker.MessageContainers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Azure.Messageing.ServiceBus.Invoker.Services
{
    public interface IMessageInvokerService
    {
        object Invoke(object container, InvocationMessage message);
        object GetMemberValue(MemberExpression exp);        
    }
}