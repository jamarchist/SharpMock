using System;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class ReplaceCall : IInterceptionStrategy
    {
        private readonly Delegate methodToCallInstead;

        public ReplaceCall(Delegate methodToCallInstead)
        {
            this.methodToCallInstead = methodToCallInstead;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.OriginalCall = methodToCallInstead;
        }
    }
}
