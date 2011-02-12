namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class InvokeOriginalCallSafe : IInterceptionStrategy
    {
        public void Intercept(IInvocation invocation)
        {
            new InvokeCallSafe(() => invocation.OriginalCall).Intercept(invocation);
        }
    }
}
