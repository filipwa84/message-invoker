using System;


namespace MessageInvoker.Shared.Services
{
    public interface IQueueConsumerService<TReceiver>
    {
        Task<TReceiver> GetMessageAsync();
        Task ProcessMethodInvocation();
        void ProcessMethodInvocation(TReceiver message);
    }
}