using System;
using System.Security.Cryptography;
using System.Text;

namespace Azure.Messageing.ServiceBus.Invoker.Helpers
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
