# RabbitMQ Message Invoker

## Overview
RabbitMQ Message Invoker is a .NET library designed to facilitate the sending and invoking methods using RabbitMQ. This library supports projects targeting .NET 8, .NET 9, and .NET Standard 2.1, making it versatile for various applications.

## DI Registration

The primary purpose of the `ServiceCollectionExtensions.cs` file is to simplify the setup and configuration of RabbitMQ message clients within an application. It includes the following key method:

- `AddServiceBusMessageInvocationClient<TFactory>(IServiceCollection services)`: This extension method registers the necessary services for RabbitMQ message invocation. It adds a singleton instance of `IChannelFactory` and `IRabbitMqMessageClient` to the service collection. The `TFactory` type parameter specifies the implementation of `IChannelFactory` to be used.

To use this extension method, call it during the service configuration phase in your application, typically in the `Startup.cs` or `Program.cs` file:

## Custom IChannelFactory Implementation

Users need to provide their own implementation of the `IChannelFactory` interface to obtain a channel. This has been left as an interface so that users can choose whether they want to reuse channels, create new ones, or adopt a pooling pattern. This flexibility allows users to optimize the channel management according to their specific requirements.

By using this extension method, you ensure that the necessary RabbitMQ services are properly configured and available for dependency injection throughout your application.
