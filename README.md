# Servicebus Message Invoker

Send any method call to the ServiceBus queue and invoke it on either a WebJob or Azure Function in a fire-and-forget fashion without having to write a specific message handler. 

It requires that the services are registered in DI container, both in the main application as well as in the WebJob/Function.

## Registration

```
using Azure.Messaging.ServiceBus.Invoker.Client;
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
        _messageInvocationClient.GetProducer("name-of-my-queue")
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
private readonly IMessageInvocationClient _messageInvocationClient;
private readonly IServiceProvider _serviceProvider;        

public Functions(IRemoteExecutionClient remoteExecutionClient, IServiceProvider serviceProvider)
{
    this._remoteExecutionClient = remoteExecutionClient;
    this._serviceProvider = serviceProvider;                
}

private async Task ProcessMessage([ServiceBusTrigger("name-of-my-queue")] ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
{
    try
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        _messageInvocationClient.GetConsumer("name-of-my-queue").ProcessMethodInvocation(scope.ServiceProvider, message);
        await serviceBusMessageActions.CompleteMessageAsync(message);                
    }
    catch (Exception e)
    {                
        Console.WriteLine($"Message {message.MessageId} was not completed");
        
        Console.WriteLine($"Reason: {e.Message}");
        Console.WriteLine($"Exception: {e.ToString()}");
        
        throw;
    }
}
```
