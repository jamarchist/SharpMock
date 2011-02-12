using System;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class InvokeReplacementCall : IInterceptionStrategy
    {
        private readonly Function<Delegate> replacementMethodBinder;

        public InvokeReplacementCall(Function<Delegate> replacementMethodBinder)
        {
            this.replacementMethodBinder = replacementMethodBinder;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.OriginalCall = replacementMethodBinder();
        }
    }
}
