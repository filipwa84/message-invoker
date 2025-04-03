using System;

namespace MessageInvoker.Shared.Helpers
{
    internal interface IHasher
    {
        string ComputeHash(byte[] plainTextBytes);
    }
}