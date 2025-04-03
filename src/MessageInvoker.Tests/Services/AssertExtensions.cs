using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ass = NUnit.Framework.Assert;

namespace MessageInvoker.Tests.Services
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual, string? message = null)
        {
            if (message is null)
            {
                Ass.That(actual, Is.EqualTo(expected));
            }
            else
            {
                Ass.That(actual, Is.EqualTo(expected), message);
            }
        }

    }
}
