using System;
using System.Collections.Generic;

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

            var arguments = new List<object>(invocation.Arguments);
            var parameters = call.Method.GetParameters();

            var truncatedArguments = arguments.GetRange(0, parameters.Length);
            invocation.Return = call.DynamicInvoke(truncatedArguments.ToArray());
        }
    }
}
