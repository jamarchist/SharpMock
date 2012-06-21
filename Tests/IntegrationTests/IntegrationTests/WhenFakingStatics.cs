using System;
using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;
using TestUtilities;

namespace IntegrationTests.IntegrationTests
{
    [TestFixture]
    public class WhenFakingStatics : SharpMockTests
    {
        [Test]
        public void ReplacementMethodIsInvoked()
        {
            var wasCalled = false;

            Replace.CallsTo(() => StaticClass.VoidReturnNoParameters()).With(() => wasCalled = true);

            var code = new CodeUnderTest();
            code.CallsVoidReturnNoParameters();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ReplacementMethodWithReturnValueIsInvoked()
        {
            Replace.CallsTo(() => StaticClass.StringReturnNoParameters()).With(() => "Interception Result");

            var code = new CodeUnderTest();
            var result = code.CallsStringReturnNoParameters();

            Assert.AreEqual("Interception Result", result);
        }

        [Test, ExpectedException(typeof(AssertionFailedException))]
        public void AssertionExceptionIsThrownIfAssertionFails()
        {
            Replace.CallsTo(() => StaticClass.StringReturnOneParameter(1))
                .Asserting((int x) => x == 77)
                .With((int x) => x.ToString());

            var code = new CodeUnderTest();
            code.CallsStringReturnOneParameter();
        }

        [Test]
        public void AssertionExceptionIsNotThrownIfAssertionPasses()
        {
            Replace.CallsTo(() => StaticClass.StringReturnOneParameter(1))
                .With((int x) => x.ToString())
                .Asserting((int x) => x == 999);

            var code = new CodeUnderTest();
            code.CallsStringReturnOneParameter();
        }

        [Test, ExpectedException(
            ExpectedException = typeof(InvalidOperationException),
            ExpectedMessage = "I threw this from a replacement.")]
        public void CustomExceptionCanBeThrownFromReplacement()
        {
            Replace.CallsTo(() => StaticClass.VoidReturnNoParameters())
                .With(() => { throw new InvalidOperationException("I threw this from a replacement."); });

            var code = new CodeUnderTest();
            code.CallsVoidReturnNoParameters();
        }

        [Test, ExpectedException(typeof(AssertionFailedException))]
        public void MultipleAssertionsAreAllowed()
        {
            Replace.CallsTo(() => StaticClass.StringReturnOneParameter(1))
                .Asserting((int x) => x > 10)
                .Asserting((int x) => x > 1000)
                .With((int x) => "dummy return value");

            var code = new CodeUnderTest();
            code.CallsStringReturnOneParameter();
        }

        [Test, ExpectedException(typeof(AssertionFailedException))]
        public void MultipleAssertionsAreAllowedInVaryingOrder()
        {
            Replace.CallsTo(() => StaticClass.StringReturnOneParameter(1))
                .Asserting((int x) => x > 1000)
                .Asserting((int x) => x > 10)
                .With((int x) => "dummy return value");

            var code = new CodeUnderTest();
            code.CallsStringReturnOneParameter();
        }

        [Test]
        public void CallsThatArentSpecifiedArentIntercepted()
        {
            Replace.CallsTo(() => StaticClass.StringReturnNoParameters()).With(() => "Intercepted.");

            var code = new CodeUnderTest();
            var result = code.CallsTwoMethods();

            Assert.AreEqual("Intercepted.", result.FirstValue);
            Assert.AreEqual("|| Original method return value when passed '9876'. ||", result.SecondValue);
        }

        [Test]
        public void MultipleOverloadsAreInterceptedWhenSpecified()
        {
            Replace.CallsTo(() => StaticClass.Overloaded()).AndAllOverloads().With(() => { });

            var code = new CodeUnderTest();
            code.CallsTwoOverloads();
        }

        [Test]
        public void CanIncludeInvocationWhenSpecified()
        {
            Replace.CallsTo(() => StaticClass.StringReturnOneParameter(0))
                .With((IInvocation i) =>
                          {
                              i.Return = i.OriginalCallInfo.Name;
                          })
                .AsInterceptor();

            var code = new CodeUnderTest();
            var result = code.CallsStringReturnOneParameter();
        
            Assert.AreEqual("StringReturnOneParameter", result);
        }
    }
}
