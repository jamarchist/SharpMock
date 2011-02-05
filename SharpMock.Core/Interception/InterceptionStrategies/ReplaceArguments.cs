using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class ReplaceArguments : IInterceptionStrategy
    {
        public delegate IList<object> ArgumentValuesReplacementFunction(IList<object> originalArguments);

        private readonly ArgumentValuesReplacementFunction replacementFunction;

        public ReplaceArguments(ArgumentValuesReplacementFunction replacementFunction)
        {
            this.replacementFunction = replacementFunction;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Arguments = replacementFunction(invocation.Arguments);
        }
    }
}