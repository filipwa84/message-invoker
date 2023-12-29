using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azure.Messageing.ServiceBus.Invoker.Helpers
{
    internal static class StringExtensions
    {
        internal static string After(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
            {
                return "";
            }
            return value.Substring(adjustedPosA);
        }

        internal static string TrimOnKeyword(this string str, string keyword)
        {
            return str.Substring(0, str.IndexOf(keyword));
        }

        internal static string RemoveFirstCaller(this string str)
        {
            var callers = str.Split('.').ToList();
            callers.RemoveAt(0);
            callers.RemoveAt(callers.Count - 1);            
            return string.Join(".", callers);
        }
    }
}
