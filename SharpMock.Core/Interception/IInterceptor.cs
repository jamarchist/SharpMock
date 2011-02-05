using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception
{
    public interface IInterceptor
    {
        bool ShouldIntercept(MethodInfo method, IList<object> arguments);
        void Intercept(IInvocation invocation);
    }
}
