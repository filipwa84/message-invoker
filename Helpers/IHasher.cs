using System;

namespace Azure.Messageing.ServiceBus.Invoker.Helpers
{
    internal interface IHasher
    {
        string ComputeHash(byte[] plainTextBytes);
    }
}