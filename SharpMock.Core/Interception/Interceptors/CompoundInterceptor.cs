using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class CompoundInterceptor : IInterceptor
    {
        private readonly IInterceptor[] interceptors;
        private MethodInfo interceptedMethod;

        public CompoundInterceptor(params IInterceptor[] interceptors)
        {
            this.interceptors = interceptors;
        }

        public bool ShouldIntercept(MethodInfo method)
        {
            interceptedMethod = method;
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            foreach (var interceptor in interceptors)
            {
                if (interceptor.ShouldIntercept(interceptedMethod))
                {
                    interceptor.Intercept(invocation);    
                }
            }
        }
    }
}