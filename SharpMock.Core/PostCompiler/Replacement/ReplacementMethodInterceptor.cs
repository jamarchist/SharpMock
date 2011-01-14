using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SharpMock.Core.DelegateTypes;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementMethodInterceptor : IInterceptor
    {
        private readonly Delegate methodToCallInstead;

        public ReplacementMethodInterceptor(Delegate methodToCallInstead)
        {
            this.methodToCallInstead = methodToCallInstead;
        }

        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.OriginalCall = methodToCallInstead;
            //var argumentArray = new List<object>(invocation.Arguments).ToArray();
            //invocation.Return = methodToCallInstead.DynamicInvoke(argumentArray);
        }
    }
}
