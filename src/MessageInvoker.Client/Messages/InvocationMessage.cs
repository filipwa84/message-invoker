using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Azure.Messaging.ServiceBus.Invoker.Messages
{
    [Serializable]
    public class InvocationMessage<TTarget> : InvocationMessage
    {
        public InvocationMessage(KeyValuePair<Type, object>[] parameters, string methodName, string callers, string tag = "")
        {
            Parameters = parameters;
            TargetType = typeof(TTarget).ToString();
            MethodName = methodName;
            Callers = callers;
            Tag = tag;
        }
    }

    [Serializable]
    public class InvocationMessage : IInvocationMessage
    {
        public string MethodName { get; set; }
        public string TargetType { get; set; }
        public string Callers { get; set; }
        public string Tag { get; set; }
        public KeyValuePair<Type, object>[] Parameters { get; set; }


        [JsonConstructor]
        public InvocationMessage()
        {

        }

        public InvocationMessage(string targetType, string methodName, object[] parameters, string tag = "")
        {
            Parameters = parameters.Select(x => new KeyValuePair<Type, object>(x.GetType(), x)).ToArray();
            TargetType = targetType;
            MethodName = methodName;
            Callers = string.Empty;
            Tag = tag;
        }
    }
}
