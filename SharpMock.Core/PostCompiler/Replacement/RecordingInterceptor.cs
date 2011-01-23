using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SharpMock.Core.DelegateTypes;

namespace SharpMock.Core.PostCompiler.Replacement
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
