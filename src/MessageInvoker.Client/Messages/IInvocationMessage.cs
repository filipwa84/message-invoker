using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Azure.Messaging.ServiceBus.Invoker.Messages
{
    public interface IInvocationMessage
    {
        string MethodName { get; set; }
        string TargetType { get; set; }
        KeyValuePair<Type, object>[] Parameters { get; set; }

    }
}