namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class InvokeOriginalCall : IInterceptionStrategy
    {
        public void Intercept(IInvocation invocation)
        {
            new InsteadOfCall(() => invocation.OriginalCall).Intercept(invocation);
        }
    }
}
