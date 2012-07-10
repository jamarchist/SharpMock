using System.Collections.Generic;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class ReplaceOutRefArguments : IInterceptionStrategy
    {
        private readonly object[] outRefArguments;

        public ReplaceOutRefArguments(object[] outRefArguments)
        {
            this.outRefArguments = outRefArguments;
        }

        public void Intercept(IInvocation invocation)
        {
            if (outRefArguments != null)
            {
                invocation.Arguments = new List<object>(outRefArguments);
            }
        }
    }
}