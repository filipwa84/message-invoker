using System;

namespace Azure.Messaging.ServiceBus.Invoker.Helpers
{
    internal interface IHasher
    {
        string ComputeHash(byte[] plainTextBytes);
    }
}