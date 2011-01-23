using System.Reflection;

namespace SharpMock.Core.Interception
{
    public interface IInterceptor
    {
        bool ShouldIntercept(MethodInfo method);
        void Intercept(IInvocation invocation);
    }
}
