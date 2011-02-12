namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class InvokeOriginalCall : IInterceptionStrategy
    {
        public void Intercept(IInvocation invocation)
        {
            new InvokeCall(() => invocation.OriginalCall).Intercept(invocation);
        }
    }
}
