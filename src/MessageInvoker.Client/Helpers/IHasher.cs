using System;

namespace Azure.Messageing.ServiceBus.Invoker.Client.Helpers
{
    internal interface IHasher
    {
        string ComputeHash(byte[] plainTextBytes);
    }
}