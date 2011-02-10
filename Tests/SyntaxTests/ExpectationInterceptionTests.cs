using NUnit.Framework;
using ScenarioDependencies;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;

namespace SyntaxTests
{
    [TestFixture]
    public class ExpectationInterceptionTests
    {
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
    }
}
