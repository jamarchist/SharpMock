using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class RecordingInterceptor : IInterceptor
    {
        public bool ShouldIntercept(MethodInfo method, IList<object> arguments)
        {
            throw new NotImplementedException();
        }

        public void Intercept(IInvocation invocation)
        {
            
        }
    }
}
