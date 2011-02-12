using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class InvokeOriginalCallSafe : IInterceptionStrategy
    {
        public void Intercept(IInvocation invocation)
        {
            var arguments = new List<object>(invocation.Arguments);
            var parameters = invocation.OriginalCall.Method.GetParameters();

            var truncatedArguments = arguments.GetRange(0, parameters.Length);
            invocation.Return = invocation.OriginalCall.DynamicInvoke(truncatedArguments.ToArray());
        }
    }
}
