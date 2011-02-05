using System;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class ReplacementMethodInterceptor : IInterceptor
    {
        private readonly Delegate methodToCallInstead;

        public ReplacementMethodInterceptor(Delegate methodToCallInstead)
        {
            this.methodToCallInstead = methodToCallInstead;
        }

        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.OriginalCall = methodToCallInstead;
        }
    }
}
