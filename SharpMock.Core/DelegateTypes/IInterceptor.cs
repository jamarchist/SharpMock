using System.Reflection;

namespace SharpMock.Core.DelegateTypes
{
    public interface IInterceptor
    {
        bool ShouldIntercept(MethodInfo method);
        void Intercept(IInvocation invocation);
    }
}
