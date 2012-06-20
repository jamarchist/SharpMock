using System;
using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core;
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

            var fake = new Faker();
            fake.CallsTo(() => StaticClass.VoidReturnNoParameters()).ByReplacingWith(() => wasCalled = true);

            var code = new CodeUnderTest();
            code.CallsVoidReturnNoParameters();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void ReplacementMethodWithReturnValueIsInvoked()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnNoParameters()).ByReplacingWith(() => "Interception Result");

            var code = new CodeUnderTest();
            var result = code.CallsStringReturnNoParameters();

            Assert.AreEqual("Interception Result", result);
        }

        [Test, ExpectedException(typeof(AssertionFailedException))]
        public void AssertionExceptionIsThrownIfAssertionFails()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnOneParameter(1))
                .Asserting((int x) => x == 77)
                .ByReplacingWith((int x) => x.ToString());

            var code = new CodeUnderTest();
            code.CallsStringReturnOneParameter();
        }

        [Test]
        public void AssertionExceptionIsNotThrownIfAssertionPasses()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnOneParameter(1))
                .ByReplacingWith((int x) => x.ToString())
                .Asserting((int x) => x == 999);

            var code = new CodeUnderTest();
            code.CallsStringReturnOneParameter();
        }

        [Test, ExpectedException(
            ExpectedException = typeof(InvalidOperationException),
            ExpectedMessage = "I threw this from a replacement.")]
        public void CustomExceptionCanBeThrownFromReplacement()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.VoidReturnNoParameters())
                .ByReplacingWith(() => { throw new InvalidOperationException("I threw this from a replacement."); });

            var code = new CodeUnderTest();
            code.CallsVoidReturnNoParameters();
        }

        [Test, ExpectedException(typeof(AssertionFailedException))]
        public void MultipleAssertionsAreAllowed()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnOneParameter(1))
                .Asserting((int x) => x > 10)
                .Asserting((int x) => x > 1000)
                .ByReplacingWith((int x) => "dummy return value");

            var code = new CodeUnderTest();
            code.CallsStringReturnOneParameter();
        }

        [Test, ExpectedException(typeof(AssertionFailedException))]
        public void MultipleAssertionsAreAllowedInVaryingOrder()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnOneParameter(1))
                .Asserting((int x) => x > 1000)
                .Asserting((int x) => x > 10)
                .ByReplacingWith((int x) => "dummy return value");

            var code = new CodeUnderTest();
            code.CallsStringReturnOneParameter();
        }

        [Test]
        public void CallsThatArentSpecifiedArentIntercepted()
        {
            var fake = new Faker();

            fake.CallsTo(() => StaticClass.StringReturnNoParameters()).ByReplacingWith(() => "Intercepted.");

            var code = new CodeUnderTest();
            var result = code.CallsTwoMethods();

            Assert.AreEqual("Intercepted.", result.FirstValue);
            Assert.AreEqual("|| Original method return value when passed '9876'. ||", result.SecondValue);
        }

        [Test]
        public void MultipleOverloadsAreInterceptedWhenSpecified()
        {
            var fake = new Faker();

            fake.CallsTo(() => StaticClass.Overloaded()).AndAllMatchingCalls().ByReplacingWith(() => { });

            var code = new CodeUnderTest();
            code.CallsTwoOverloads();
        }
    }
}
