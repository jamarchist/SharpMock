namespace SharpMock.Core.Interception
{
    public interface IInterceptionStrategy
    {
        void Intercept(IInvocation invocation);
    }
}