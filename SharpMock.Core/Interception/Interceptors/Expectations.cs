using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.Interceptors
{
    public class Expectations
    {
        private readonly MethodInfo method;
        private readonly IList<object> arguments;

        public Expectations(MethodInfo method, IList<object> arguments)
        {
            this.method = method;
            this.arguments = arguments;
        }

        public MethodInfo Method
        {
            get { return method; }
        }

        public IList<object> Arguments
        {
            get { return arguments; }
        }

        public System.Delegate Assertion { get; set; }
        public System.Delegate Replacement { get; set; }
    }
}