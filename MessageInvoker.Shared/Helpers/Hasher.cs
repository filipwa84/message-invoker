using System;
using System.Security.Cryptography;
using System.Text;

namespace MessageInvoker.Shared.Helpers
{
    public class Hasher : IHasher
    {
        public string ComputeHash(byte[] plainTextBytes)
        {
            var hash = new MD5CryptoServiceProvider();

            var hashBytes = hash.ComputeHash(plainTextBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
