using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SharpMock.Core.DelegateTypes;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReturnValueInterceptor : IInterceptor
    {
        public delegate object ReturnValueReplacementFunction(object originalReturnValue);

        private readonly ReturnValueReplacementFunction replacementFunction;

        public ReturnValueInterceptor(ReturnValueReplacementFunction replacementFunction)
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
