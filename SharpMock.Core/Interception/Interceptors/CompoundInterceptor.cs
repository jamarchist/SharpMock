using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class CompoundInterceptor : IInterceptor
    {
        private readonly IMatchingStrategy matcher;
        private readonly IInterceptor[] interceptors;

        public CompoundInterceptor(IMatchingStrategy matcher, params IInterceptor[] interceptors)
        {
            this.interceptors = interceptors;
            this.matcher = matcher;
        }

        public bool ShouldIntercept(MethodInfo method)
        {
            return matcher.Matches(method);
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