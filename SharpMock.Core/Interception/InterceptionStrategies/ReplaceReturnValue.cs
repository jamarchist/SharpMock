using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class ReplaceReturnValue : IInterceptionStrategy
    {
        public delegate object ReturnValueReplacementFunction(object originalReturnValue);

        private readonly ReturnValueReplacementFunction replacementFunction;

        public ReplaceReturnValue(ReturnValueReplacementFunction replacementFunction)
        {
            this.replacementFunction = replacementFunction;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Return = replacementFunction(invocation.Return);
        }
    }
}
