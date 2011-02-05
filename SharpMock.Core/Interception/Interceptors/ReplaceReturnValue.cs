using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class ReplaceReturnValue : IInterceptor
    {
        public delegate object ReturnValueReplacementFunction(object originalReturnValue);

        private readonly ReturnValueReplacementFunction replacementFunction;

        public ReplaceReturnValue(ReturnValueReplacementFunction replacementFunction)
        {
            this.replacementFunction = replacementFunction;
        }

        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            //var argumentArray = new List<object>(invocation.Arguments).ToArray();
            //var orignalReturnValue = invocation.OriginalCall.DynamicInvoke(argumentArray);
            invocation.Return = replacementFunction(invocation.Return);
        }
    }
}
