using System;
using System.Collections.Generic;
using System.Text;
using SharpMock.Core.Interception.Helpers;

namespace SharpMock.Core.Interception.InterceptionStrategies
{
    public class Assert : IInterceptionStrategy
    {
        private readonly Function<Delegate> assertionBinder;

        public Assert(Function<Delegate> assertionBinder)
        {
            this.assertionBinder = assertionBinder;
        }

        public void Intercept(IInvocation invocation)
        {
            var assertion = assertionBinder();
            if (assertion != null)
            {
                var assertionPassed = (bool) assertion.SafeInvoke(invocation.Arguments);

                if (!assertionPassed)
                {
                    throw new AssertionFailedException();
                }
            }
        }
    }
}
