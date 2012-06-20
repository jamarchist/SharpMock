using System;
using System.Collections.Generic;
using System.Reflection;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    /// <summary>
    /// Invokes a call using the current argument list
    /// and sets the current return value to the result
    /// </summary>
    public class InsteadOfCall : IInterceptionStrategy
    {
        private readonly Function<Delegate> callBinder;

        public InsteadOfCall(Function<Delegate> callBinder)
        {
            this.callBinder = callBinder;
        }

        public void Intercept(IInvocation invocation)
        {
            var arguments = new List<object>(invocation.Arguments).ToArray();

            try
            {
                invocation.Return = callBinder().SafeInvoke(arguments);
            }
            catch (TargetInvocationException exceptionOnInvoke)
            {
                throw exceptionOnInvoke.InnerException;
            }
        }
    }
}
