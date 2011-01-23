using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class RecordingInterceptor : IInterceptor
    {
        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }

        public void Intercept(IInvocation invocation)
        {
            
        }
    }
}
