using System;
using System.Collections.Generic;
using System.Text;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class Assert : IInterceptionStrategy
    {
        private readonly Function<IList<Delegate>> assertionsBinder;

        public Assert(Function<IList<Delegate>> assertionsBinder)
        {
            this.assertionsBinder = assertionsBinder;
        }

        public void Intercept(IInvocation invocation)
        {
            var assertions = assertionsBinder();
            
            foreach (var individualAssertion in assertions)
            {
                var assertionPassed = (bool)individualAssertion.SafeInvoke(invocation.Arguments);

                if (!assertionPassed)
                {
                    throw new AssertionFailedException();
                }    
            }
        }
    }
}
