using System;

namespace SharpMock.Core.Interception.Interceptors
{
    public class RecordingInterceptor : IInterceptor
    {
        private Expectations expectiations;

        public bool ShouldIntercept(IInvocation invocation)
        {
            expectiations = new Expectations(invocation.OriginalCallInfo, invocation.Arguments);
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            //// Set a default return value. It won't be used,
            //// but a value type will have to be returned to avoid a cast error
            var returnType = invocation.OriginalCall.Method.ReturnType;
            if (!returnType.Equals(typeof(void)) && returnType.IsValueType)
            {
                invocation.Return = Activator.CreateInstance(returnType);
            }
        }

        public Expectations GetExpectations()
        {
            return expectiations;
        }
    }
}
