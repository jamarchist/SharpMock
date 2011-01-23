using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
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