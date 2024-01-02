namespace Azure.Messageing.ServiceBus.Invoker.Tests.Services
{
    public interface IFakeService
    {
        void StringParameterMethod(string input);
        void BoolParameterMethod(bool input);
        bool BooleanReturnedMethod();

        NestedService Nested { get; }
    }
}