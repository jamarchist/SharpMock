using System.Collections.Generic;
using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.Syntax;
using TestUtilities;

namespace IntegrationTests.IntegrationTests
{
    [TestFixture]
    public class WhenFakingConcreteClasses : SharpMockTests
    {
        [Test]
        public void SealedClassMethodIsIntercepted()
        {
            var fake = new Faker();
            var wasCalled = false;

            fake.CallsTo((SealedClass s) => s.VoidReturnNoParameters()).ByReplacingWith(() => { wasCalled = true; });

            var code = new CodeUnderTest();
            code.CallsSealedMethod();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void SealedClassMethodWithParametersIsIntercepted()
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

        [Test]
        public void NonVirtualMethodOnNonSealedClassIsIntercepted()
        {
            var fake = new Faker();
            var wasCalled = false;

            fake.CallsTo((SomeConcreteClass i) => i.SomeMethod())
                .ByReplacingWith(() => { wasCalled = true; });

            var code = new CodeUnderTest();
            code.CallsSomeConcreteClassMethod(null);

            Assert.IsTrue(wasCalled);
        }
    }

    public class InterceptConcreteClassMethodSpecification : IReplacementSpecification
    {
        public IList<ReplaceableMethodInfo> GetMethodsToReplace()
        {
            var type = typeof(SomeConcreteClass);
            var method = type.GetMethod("SomeMethod");

            var replaceable = method.AsReplaceable();

            return new List<ReplaceableMethodInfo> { replaceable };
        }
    }
}
