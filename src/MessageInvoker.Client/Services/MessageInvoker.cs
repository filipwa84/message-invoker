using Azure.Messageing.ServiceBus.Invoker.Client.MethodTransporters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Messageing.ServiceBus.Invoker.Client.Services
{
    internal class MessageInvoker : IMessageInvoker
    {
        private readonly IServiceProvider _serviceProvider;
        public MessageInvoker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Invoke(InvocationMessage message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var targetType = GetTypeFromAssemblies(message.TargetType);

                var service = scope.ServiceProvider.GetService(targetType);

                var methodParams = GetParameterArray(message.Parameters);

                if (string.IsNullOrEmpty(message.Callers))
                {
                    return InvokeMethod(service, methodParams, message.MethodName);
                }
                else
                {
                    var propValue = service;
                    foreach (var caller in message.Callers.Split('.'))
                    {
                        var propInfo = propValue.GetType().GetProperty(caller);
                        propValue = propInfo.GetValue(propValue);
                    }

                    return InvokeMethod(propValue, methodParams, message.MethodName);
                }
            }
        }

        public object GetMemberValue(MemberExpression exp)
        {
            if (exp.Expression is ConstantExpression)
            {
                return ((ConstantExpression)exp.Expression).Value.GetType().GetField(exp.Member.Name).GetValue(((ConstantExpression)exp.Expression).Value);
            }
            else if (exp.Expression is MemberExpression memberExpression)
            {
                return GetMemberValue((MemberExpression)exp.Expression);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private Type GetTypeFromAssemblies(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var foundType = assembly.GetType(typeName);

                if (foundType != null)
                    return foundType;
            }

            return null;
        }

        private object[] GetParameterArray(KeyValuePair<Type, object>[] parameters)
        {
            var objParams = new List<object>();

            foreach (var p in parameters)
            {
                if (p.Value is JObject || p.Value is JArray || p.Key == typeof(int) || p.Key == typeof(long) || p.Key == typeof(uint) || p.Key == typeof(byte))
                {
                    objParams.Add(JsonConvert.DeserializeObject(p.Value.ToString(), p.Key));
                }
                else
                {
                    objParams.Add(p.Value);
                }
            }
            return objParams.ToArray();
        }

        private object InvokeMethod(object obj, object[] methodParams, string methodName)
        {
            var invocation = obj.GetType().GetMethod(methodName)?.Invoke(obj, methodParams);

            if (!(invocation is Task)) return invocation;

            Task.Run(async () => await (Task)invocation).GetAwaiter().GetResult();

            return null;
        }
    }
}
