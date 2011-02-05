using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class ReplaceArguments : IInterceptor
    {
        public delegate IList<object> ArgumentValuesReplacementFunction(IList<object> originalArguments);

        private readonly ArgumentValuesReplacementFunction replacementFunction;

        public ReplaceArguments(ArgumentValuesReplacementFunction replacementFunction)
        {
            this.replacementFunction = replacementFunction;
        }

        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Arguments = replacementFunction(invocation.Arguments);
            //var replacementArguments = replacementFunction(invocation.Arguments);
            //invocation.Return = invocation.OriginalCall.DynamicInvoke(new List<object>(replacementArguments).ToArray());
        }
    }
}