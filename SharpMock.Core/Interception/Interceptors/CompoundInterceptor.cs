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

        public bool ShouldIntercept(MethodInfo method, IList<object> arguments)
        {
            return matcher.Matches(method, arguments);
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