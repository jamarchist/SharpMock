using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Syntax;

namespace IntegrationTests
{
    [TestFixture]
    public class SealedMethodTests
    {
        [Test]
        public void InterceptsSealedSpecification()
        {
            var fake = new Faker();
            var wasCalled = false;

            fake.CallsTo((SealedClass s) => s.VoidReturnNoParameters()).ByReplacingWith(() => { wasCalled = true; });

            var code = new CodeUnderTest();
            code.CallsSealedMethod();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void InterceptsSealedMethodWithParameters()
        {
            var fake = new Faker();
            string interception = "Method was not intercepted.";

            fake.CallsTo((SealedClass s) => s.StringReturnOneParameter(0)).ByReplacingWith(
                (int i) =>
                    {
                        interception = string.Format("Method was called with {0}.", i);
                        return "Fake return value.";
                    });

            var code = new CodeUnderTest();
            var result = code.CallsSealedMethodWithParameter(42);

            Assert.AreEqual("Method was called with 42.", interception);
            Assert.AreEqual("Fake return value.", result);
        }

        [Test, Ignore]
        public void InterceptsInterfaceMethodCalls()
        {
            var fake = new Faker();
            var wasCalled = false;

            fake.CallsTo((SomeConcreteClass i) => i.SomeMethod()).ByReplacingWith(() => { wasCalled = true; });

            var code = new CodeUnderTest();
            code.CallsSomeInterfaceMethod(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
