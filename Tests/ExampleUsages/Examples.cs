using System;
using ExampleApplication;
using NUnit.Framework;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.StaticReflection;
using SharpMock.Core.Syntax;

namespace ExampleUsages
{
    [TestFixture]
    public class Examples
    {
        [SetUp]
        public void ClearRegistryBefore()
        {
            InterceptorRegistry.Clear();
        }

        [TearDown]
        public void ClearRegistryAfter()
        {
            InterceptorRegistry.Clear();
        }

        [Test]
        public void NoInstrumentation()
        {
            Program.Main(new string[] { "Ryan" });
        }

        [Test]
        public void InterceptCallsInAssembly()
        {
            var intercept = new Faker();
            intercept.CallsTo(() => Dao.Insert(null)).ByReplacingWith(
                (Model m) =>
                    {
                        Console.WriteLine("Stopping insert.");
                        return 99;
                    });

            Program.Main(new string[] { "Hello" });
        }

        [Test]
        public  void AddInstrumentationSpec()
        {
            var spec = new InstrumentationSpec();
            spec.SpecifyInterceptors(new SpecificationRegistry());

            Program.Main(new string[] { "Instrumented" });
        }

        private class InstrumentationSpec : IInterceptionSpecification
        {
            public void SpecifyInterceptors(ISpecificationRegistry registry)
            {
                var matchDaoInsert = new AllOverloadsMatch(Method.Of<Model, int>(Dao.Insert));
                VoidAction<IInvocation> before = i => Console.WriteLine("<< Entering Dao.Insert >>");
                VoidAction<IInvocation> after = i => Console.WriteLine("<< Exiting Dao.Insert >>");

                var surround = new CompoundInterceptor(matchDaoInsert,
                    new InvokeWithInvocation(() => before),
                    new InvokeOriginalCall(),
                    new InvokeWithInvocation(() => after));

                registry.AddInterceptor(surround);
            }
        }
    }
}
