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
