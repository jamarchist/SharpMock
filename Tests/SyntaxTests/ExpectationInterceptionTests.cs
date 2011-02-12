using NUnit.Framework;
using ScenarioDependencies;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;

namespace SyntaxTests
{
    [TestFixture]
    public class ExpectationInterceptionTests
    {
        [TearDown]
        public void ClearRegistry()
        {
            InterceptorRegistry.Clear();
        }

        [Test]
        public void InterceptsStaticExpectation()
        {
            var fake = new Faker();

            fake.CallsTo(() => StaticClass.StringReturnNoParameters());
        }

        [Test]
        public void RecordsExpectation()
        {
            var fake = new Faker();

            fake.CallsTo(() => StaticClass.StringReturnNoParameters());

            var recorder = InterceptorRegistry.GetCurrentRecorder();

            Assert.AreEqual(0, recorder.GetExpectations().Arguments.Count);
            Assert.AreEqual("StringReturnNoParameters", recorder.GetExpectations().Method.Name);
        }

        [Test, ExpectedException(typeof(MethodNotInterceptedException))]
        public void DoesNotInterceptStaticMethodThatIsntExplicitlyFaked()
        {
            StaticClass.VoidReturnNoParameters();
        }

        [Test]
        public void RecordsCallingAssertion()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnNoParameters()).Asserting(() => true == true);

            var recorder = InterceptorRegistry.GetCurrentRecorder();

            var expectedAction = recorder.GetExpectations().Assertions;
            var result = expectedAction.DynamicInvoke(null);

            Assert.IsTrue((bool)result);
        }

        [Test]
        public void RecordsReplacement()
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
