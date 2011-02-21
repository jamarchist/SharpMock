using System;
using System.Collections.Generic;
using System.Reflection;
using SharpMock.Core.Interception.InterceptionStrategies;

namespace SharpMock.Core.Interception.Interceptors
{
    public class RegistryInterceptor : IInterceptor
    {
        private MethodInfo interceptedMethod;
        private IList<object> interceptedArguments;

        public bool ShouldIntercept(MethodInfo method, IList<object> arguments)
        {
            interceptedMethod = method;
            interceptedArguments = arguments;

            // It doesn't matter what we return here
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            var wasIntercepted = false;
            foreach (var interceptor in InterceptorRegistry.GetInterceptors())
            {
                if (interceptor.ShouldIntercept(interceptedMethod, interceptedArguments))
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
