using System.Collections.Generic;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class ReplaceArguments : IInterceptionStrategy
    {
        private readonly Function<Function<IList<object>, IList<object>>> replacementMethodBinder;

        public ReplaceArguments(Function<Function<IList<object>, IList<object>>> replacementMethodBinder)
        {
            this.replacementMethodBinder = replacementMethodBinder;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Arguments = replacementMethodBinder()(invocation.Arguments);
        }
    }
}