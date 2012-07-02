namespace SharpMock.Core.Interception
{
    public interface IInterceptor
    {
        bool ShouldIntercept(IInvocation invocation);
        void Intercept(IInvocation invocation);
    }
}
