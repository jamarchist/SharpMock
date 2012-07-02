using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class CompoundInterceptor : IInterceptor
    {
        private readonly IMatchingStrategy matcher;
        private readonly IInterceptionStrategy[] interceptors;

        public CompoundInterceptor(IMatchingStrategy matcher, params IInterceptionStrategy[] interceptors)
        {
            this.interceptors = interceptors;
            this.matcher = matcher;
        }

        public bool ShouldIntercept(IInvocation invocation)
        {
            return matcher.Matches(invocation.OriginalCallInfo, invocation.Arguments);
        }

        public void Intercept(IInvocation invocation)
        {
            foreach (var interceptor in interceptors)
            {
                interceptor.Intercept(invocation);   
            }
        }
    }
}