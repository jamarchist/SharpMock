using System;
using System.Collections.Generic;

namespace SharpMock.Core.Interception.Helpers
{
    public static class InvocationHelper
    {
        public static object SafeInvoke(this Delegate method, IList<object> arguments)
        {
            var args = new List<object>(arguments);
            var parameters = method.Method.GetParameters();

            var truncatedArguments = args.GetRange(0, parameters.Length);
            return method.DynamicInvoke(truncatedArguments.ToArray());
        }
    }
}
