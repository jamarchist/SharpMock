using NUnit.Framework;
using ScenarioDependencies;
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

        [Test, ExpectedException(typeof(MethodNotInterceptedException))]
        public void DoesNotInterceptStaticMethodThatIsntExplicitlyFaked()
        {
            StaticClass.VoidReturnNoParameters();
        }
    }
}
