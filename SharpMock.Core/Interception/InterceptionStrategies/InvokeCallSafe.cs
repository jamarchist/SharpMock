using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class InvokeCallSafe : IInterceptor
    {
        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            var arguments = new List<object>(invocation.Arguments);
            var parameters = invocation.OriginalCall.Method.GetParameters();

            var truncatedArguments = arguments.GetRange(0, parameters.Length);
            invocation.Return = invocation.OriginalCall.DynamicInvoke(truncatedArguments.ToArray());
        }
    }
}
