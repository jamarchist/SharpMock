using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class InvokeWithInvocation : IInterceptionStrategy
    {
        private readonly Function<Delegate> callBinder;

        public InvokeWithInvocation(Function<Delegate> callBinder)
        {
            this.callBinder = callBinder;
        }

        public void Intercept(IInvocation invocation)
        {
            var argumentList = new List<object>();
            argumentList.Add(invocation);
            var arguments = argumentList.ToArray();

            try
            {
                callBinder().DynamicInvoke(arguments);
            }
            catch (TargetInvocationException exceptionOnInvoke)
            {
                throw exceptionOnInvoke.InnerException;
            }
        }
    }
}