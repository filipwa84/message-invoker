# Azure ServiceBus Message Invoker

Send any method call to the Azure ServiceBus Queue and invoke it with either a WebJob or Azure Function, in a fire-and-forget fashion without having to write specific message handlers. 

It requires that the services are registered in DI container, both in the main application as well as in the WebJob/Function.

## Registration

```c#
using Azure.Messaging.ServiceBus.Invoker;
.
.
.
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddServiceBusMessageInvocationClient("my-servicebus-connection-string");

```

## Submit message to the queue

To submit the invocation call to the queue do the following:

```c#
public class MyService : IMyService
{
    public readonly IMessageInvocationClient _messageInvocationClient;

    public MyService(IMessageInvocationClient messageInvocationClient)
    {
        _messageInvocationClient = messageInvocationClient;
    }

    public DoSomething()
    {
        //Do some work

        //Submit long running task to the queue
        _messageInvocationClient.QueueProducerByQueueName("name-of-my-queue")
          .SubmitMethodExpressionToQueue<IMyService>(x => x.SomeLongRunningTask("some-id", "some-id-tag"));
    }

    public void SomeLongRunningTask(string id)
    {
        //Some long running task
    }
}
```

## Process the message
In the WebJob or Azure Function consume and invoke the call as follows:

```c#
public class Functions
{

    private readonly IMessageInvocationClient _messageInvocationClient;        

    public Functions(IMessageInvocationClient messageInvocationClient)
    {
        _messageInvocationClient = messageInvocationClient;            
    }

    public async Task ProcessMessage([ServiceBusTrigger("name-of-my-queue")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
    {
        try
        {
            _messageInvocationClient.QueueConsumerByQueueName("name-of-my-queue").ProcessMethodInvocation(message);
            await messageActions.CompleteMessageAsync(message);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Message {message.MessageId} was not completed");

            Console.WriteLine($"Reason: {e.Message}");
            Console.WriteLine($"Exception: {e}");

            throw;
        }
    }
}
```
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
