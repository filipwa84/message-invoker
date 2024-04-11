using System;

namespace Azure.Messaging.ServiceBus.Invoker.Client.Helpers
{
    internal interface IHasher
    {
        string ComputeHash(byte[] plainTextBytes);
    }
}