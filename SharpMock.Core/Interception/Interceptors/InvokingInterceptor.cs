using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class InvokingInterceptor : IInterceptor
    {
        private readonly IMatchingStrategy matcher;

        public InvokingInterceptor(IMatchingStrategy matcher)
        {
            this.matcher = matcher;
        }

        public bool ShouldIntercept(MethodInfo method)
        {
            var matches = matcher.Matches(method);
            return matches; //matcher.Matches(method);
        }

        public void Intercept(IInvocation invocation)
        {
            var arguments = new List<object>(invocation.Arguments).ToArray();
            invocation.Return = invocation.OriginalCall.DynamicInvoke(arguments);
        }
    }
}
