using System;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class ReplaceCall : IInterceptionStrategy
    {
        private readonly Delegate methodToCallInstead;

        private readonly Function<Delegate> replacementMethodBinder;

        public ReplaceCall(Function<Delegate> replacementMethodBinder)
        {
            this.replacementMethodBinder = replacementMethodBinder;
        }

        public ReplaceCall(Delegate methodToCallInstead)
        {
            this.methodToCallInstead = methodToCallInstead;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.OriginalCall = replacementMethodBinder == null ? methodToCallInstead : replacementMethodBinder();
        }
    }
}
