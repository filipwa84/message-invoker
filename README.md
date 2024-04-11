# Servicebus Message Invoker

Send any method call to the ServiceBus queue and invoke it on either a WebJob or Azure Function in a fire-and-forget fashion. It requires that the services are registered in DI container, both in the main application as well as in the WebJob/Function.

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
```
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
        _messageInvocationClient.GetProducter("name-of-my-queue")
          .SubmitMethodExpressionToQueue<IMyService>(x => x.SomeService("some-id", "some-id"));
    }

    public void SomeLongRunningTask(string id)
    {
      .
      .
      .
    }
}
```

### Process the message
In the WebJob or Azure Function 
