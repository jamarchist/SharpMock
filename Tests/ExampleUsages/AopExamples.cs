using System;
using System.Text;
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
using TestUtilities;

namespace ExampleUsages
{
    [TestFixture]
    public class AopExamples : SharpMockTests
    {
        [Test]
        public void NoInstrumentation()
        {
            Program.Main(new string[] { "Ryan" });
        }

        [Test]
        public void InterceptCallsInAssembly()
        {
            Replace.CallsTo(() => Dao.Insert(null)).With(
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
                var surround = new CompoundInterceptor(matchDaoInsert,
                    new TraceEntry(),
                    new InvokeOriginalCall(),
                    new TraceExit());

                registry.AddInterceptor(surround);
            }
        }

        private class TraceEntry : IInterceptionStrategy
        {
            public void Intercept(IInvocation invocation)
            {
                VoidAction<IInvocation> before = i =>
                {
                    var arguments = new StringBuilder();
                    for (int index = 0; index < invocation.Arguments.Count; index++)
                    {
                        if (index > 0) arguments.Append(", ");
                        var argumentString = invocation.Arguments[index] == null ? 
                            "<null>" : invocation.Arguments[index].ToString();
                        arguments.Append(argumentString);
                    }

                    Console.WriteLine("<< Entering {0}.{1}({2}) >>", 
                        i.OriginalCallInfo.DeclaringType.Name, i.OriginalCallInfo.Name, arguments);
                };

                new InvokeWithInvocation(() => before).Intercept(invocation);
            }
        }

        private class TraceExit : IInterceptionStrategy
        {
            public void Intercept(IInvocation invocation)
            {
                VoidAction<IInvocation> after = i =>
                {
                    var returnValue = i.Return == null ? "<null>" : i.Return.ToString();
                    Console.WriteLine("<< Exiting {0}.{1} with value {2} >>", 
                        i.OriginalCallInfo.DeclaringType.Name, i.OriginalCallInfo.Name, returnValue);
                };

                new InvokeWithInvocation(() => after).Intercept(invocation);
            }
        }
    }
}
