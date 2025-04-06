using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageInvoker.Shared;
using MessageInvoker.AzureServiceBus.Services;

namespace MessageInvoker.AzureServiceBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceBusMessageInvocationClient(this IServiceCollection services, string connectionString)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.AddSingleton((Func<IServiceProvider, Services.IInvocationClient>)(provider => new InvocationClient(provider, connectionString, ServiceBusTransportType.AmqpWebSockets)));
        }

        public static IServiceCollection AddServiceBusMessageInvocationClient(this IServiceCollection services, string connectionString, ServiceBusTransportType transportType)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.AddSingleton((Func<IServiceProvider, Services.IInvocationClient>)(provider => new InvocationClient(provider, connectionString, transportType)));
        }
    }
}
