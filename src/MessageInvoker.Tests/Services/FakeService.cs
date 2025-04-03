using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageInvoker.Tests.Services
{
    public class FakeService : IFakeService
    {
        public void StringParameterMethod(string input)
        {
            Debug.WriteLine(input);
        }

        public void BoolParameterMethod(bool input)
        {
            Debug.WriteLine(input);
        }

        public bool BooleanReturnedMethod()
        {
            Debug.WriteLine($"{nameof(BooleanReturnedMethod)} executed successfully");

            return true;
        }

        public NestedService Nested { get; } = new NestedService();
    }

    public class NestedService
    {
        public void StringParameterMethod(string input)
        {
            Debug.WriteLine(input);
        }
    }
}
