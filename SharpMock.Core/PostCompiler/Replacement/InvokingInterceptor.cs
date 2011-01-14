using System.Collections.Generic;
using System.Reflection;
using SharpMock.Core.DelegateTypes;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class InvokingInterceptor : IInterceptor
    {
        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            var arguments = new List<object>(invocation.Arguments).ToArray();
            invocation.Return = invocation.OriginalCall.DynamicInvoke(arguments);
        }
    }
}
