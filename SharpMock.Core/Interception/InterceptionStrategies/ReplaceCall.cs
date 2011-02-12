using System;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class ReplaceCall : IInterceptionStrategy
    {
        private readonly Function<Delegate> replacementMethodBinder;

        public ReplaceCall(Function<Delegate> replacementMethodBinder)
        {
            this.replacementMethodBinder = replacementMethodBinder;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.OriginalCall = replacementMethodBinder();
        }
    }
}
