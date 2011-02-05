using System;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class ReplaceCall : IInterceptor
    {
        private readonly Delegate methodToCallInstead;

        public ReplaceCall(Delegate methodToCallInstead)
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
