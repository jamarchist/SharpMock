using System;
using System.Collections.Generic;
using System.Reflection;
using SharpMock.Core.Interception.InterceptionStrategies;

namespace SharpMock.Core.Interception.Interceptors
{
    public class RegistryInterceptor : IInterceptor
    {
        private IInvocation originalInvocation;

        public bool ShouldIntercept(IInvocation invocation)
        {
            originalInvocation = invocation;

            // It doesn't matter what we return here
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            var wasIntercepted = false;
            foreach (var interceptor in InterceptorRegistry.GetInterceptors())
            {
                if (interceptor.ShouldIntercept(originalInvocation))
                {
                    interceptor.Intercept(invocation);
                    wasIntercepted = true;
                }
            }

            if (!wasIntercepted)
            {
                var callOriginal = new InvokeOriginalCall();
                callOriginal.Intercept(invocation);
            }
        }
    }
}
