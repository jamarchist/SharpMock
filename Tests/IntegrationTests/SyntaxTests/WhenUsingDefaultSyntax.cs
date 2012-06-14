using NUnit.Framework;
using ScenarioDependencies;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;

namespace IntegrationTests.SyntaxTests
{
    [TestFixture]
    public class WhenUsingDefaultSyntax : SharpMockTests
    {
        [Test]
        public void StaticMethodIsIntercepted()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnNoParameters());
        }

        [Test]
        public void ExpectationIsRecorded()
        {
            var fake = new Faker();

            fake.CallsTo(() => StaticClass.StringReturnNoParameters());

            var recorder = InterceptorRegistry.GetCurrentRecorder();

            Assert.AreEqual(0, recorder.GetExpectations().Arguments.Count);
            Assert.AreEqual("StringReturnNoParameters", recorder.GetExpectations().Method.Name);
        }

        [Test, ExpectedException(typeof(MethodNotInterceptedException))]
        public void UnspecifiedStaticMethodIsNotIntercepted()
        {
            StaticClass.VoidReturnNoParameters();
        }

        [Test]
        public void AssertionIsRecorded()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnNoParameters()).Asserting(() => true);

            var recorder = InterceptorRegistry.GetCurrentRecorder();

            var expectedAction = recorder.GetExpectations().Assertions;
            var result = expectedAction[0].DynamicInvoke(null);

            Assert.IsTrue((bool)result);
        }

        [Test]
        public void ReplacementIsRecorded()
        {
            var wasCalled = false;
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnNoParameters()).ByReplacingWith(() => wasCalled = true);

            var recorder = InterceptorRegistry.GetCurrentRecorder();

            var replacementAction = recorder.GetExpectations().Replacement;
            replacementAction.DynamicInvoke(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
