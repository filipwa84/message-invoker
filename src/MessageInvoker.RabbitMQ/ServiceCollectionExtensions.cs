﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageInvoker.Shared;
using MessageInvoker.RabbitMQ.Services;


namespace MessageInvoker.RabbitMQ
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqInvocationClient<TFactory>(this IServiceCollection services) where TFactory : class, IChannelFactory
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IChannelFactory, TFactory>();

            return services.AddSingleton<IInvocationClient>(provider =>
            { 
                var channelFactory = provider.GetRequiredService<IChannelFactory>();
                return new InvocationClient(provider, channelFactory);
            });
        }
    }
}
