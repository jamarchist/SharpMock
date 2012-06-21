using NUnit.Framework;
using ScenarioDependencies;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;
using TestUtilities;

namespace IntegrationTests.SyntaxTests
{
    [TestFixture]
    public class WhenUsingDefaultSyntax : SharpMockTests
    {
        [Test]
        public void StaticMethodIsIntercepted()
        {
            Replace.CallsTo(() => StaticClass.StringReturnNoParameters());
        }

        [Test]
        public void ExpectationIsRecorded()
        {
            Replace.CallsTo(() => StaticClass.StringReturnNoParameters());

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
            Replace.CallsTo(() => StaticClass.StringReturnNoParameters()).Asserting(() => true);

            var recorder = InterceptorRegistry.GetCurrentRecorder();

            var expectedAction = recorder.GetExpectations().Assertions;
            var result = expectedAction[0].DynamicInvoke(null);

            Assert.IsTrue((bool)result);
        }

        [Test]
        public void ReplacementIsRecorded()
        {
            var wasCalled = false;
            Replace.CallsTo(() => StaticClass.StringReturnNoParameters()).With(() => wasCalled = true);

            var recorder = InterceptorRegistry.GetCurrentRecorder();

            var replacementAction = recorder.GetExpectations().Replacement;
            replacementAction.DynamicInvoke(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
