using System;
using System.Collections.Generic;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    /// <summary>
    /// Invokes a call using a truncated version of the current
    /// argument list if necessary and sets the return value to the result
    /// </summary>
    public class InvokeCallSafe : IInterceptionStrategy
    {
        private readonly Function<Delegate> callBinder;

        public InvokeCallSafe(Function<Delegate> callBinder)
        {
            this.callBinder = callBinder;
        }

        public void Intercept(IInvocation invocation)
        {
            var call = callBinder();
            invocation.Return = call.SafeInvoke(invocation.Arguments);
        }
    }
}
