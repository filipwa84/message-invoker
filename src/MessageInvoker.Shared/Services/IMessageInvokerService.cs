using MessageInvoker.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MessageInvoker.Shared.Services
{
    public interface IMessageInvoker
    {
        object Invoke(InvocationMessage message);
        object GetMemberValue(MemberExpression exp);
    }
}