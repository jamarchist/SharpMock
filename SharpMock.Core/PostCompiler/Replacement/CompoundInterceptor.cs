using System.Reflection;
using SharpMock.Core.DelegateTypes;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class CompoundInterceptor : IInterceptor
    {
        private readonly IInterceptor[] interceptors;

        public CompoundInterceptor(params IInterceptor[] interceptors)
        {
            this.interceptors = interceptors;
        }

        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            foreach (var interceptor in interceptors)
            {
                interceptor.Intercept(invocation);
            }
        }
    }
}