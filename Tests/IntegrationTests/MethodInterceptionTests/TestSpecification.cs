using System;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;
using SharpMock.Core.Interception.Registration;

namespace MethodInterceptionTests
{
    public class TestSpecification : IInterceptionSpecification
    {
        public void SpecifyInterceptors(ISpecificationRegistry registry)
        {
            VoidAction<IInvocation> interceptor = i =>
            {
                Console.WriteLine("BEFORE CALL.");
                i.Arguments[0] = 888;
            };

            var compoundInterceptor =
                new CompoundInterceptor(new AlwaysMatches(), new BeforeCall(() => interceptor));
            
            registry.AddInterceptor(compoundInterceptor);
        }
    }
}