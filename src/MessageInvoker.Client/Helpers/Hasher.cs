using System;
using System.Security.Cryptography;
using System.Text;

namespace Azure.Messaging.ServiceBus.Invoker.Client.Helpers
{
    internal class Hasher : IHasher
    {
        public string ComputeHash(byte[] plainTextBytes)
        {
            var hash = new MD5CryptoServiceProvider();

            var hashBytes = hash.ComputeHash(plainTextBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
