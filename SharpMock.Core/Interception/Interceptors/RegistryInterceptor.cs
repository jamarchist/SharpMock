using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class RegistryInterceptor : IInterceptor
    {
        private MethodInfo interceptedMethod;

        public bool ShouldIntercept(MethodInfo method)
        {
            interceptedMethod = method;

            // It doesn't matter what we return here
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            foreach (var interceptor in InterceptorRegistry.GetInterceptors())
            {
                if (interceptor.ShouldIntercept(interceptedMethod))
                {
                    interceptor.Intercept(invocation);    
                }
            }
        }
    }
}
