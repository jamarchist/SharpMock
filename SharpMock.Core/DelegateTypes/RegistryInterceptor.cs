using System;
using System.Reflection;

namespace SharpMock.Core.DelegateTypes
{
    public class RegistryInterceptor : IInterceptor
    {
        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            foreach (var interceptor in InterceptorRegistry.GetInterceptors())
            {
                interceptor.Intercept(invocation);
            }
        }
    }
}
