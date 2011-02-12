using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class InvokeOriginalCall : IInterceptionStrategy
    {
        public void Intercept(IInvocation invocation)
        {
            var arguments = new List<object>(invocation.Arguments).ToArray();
            invocation.Return = invocation.OriginalCall.DynamicInvoke(arguments);
        }
    }
}
