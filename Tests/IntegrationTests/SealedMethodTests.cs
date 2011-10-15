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
    }
}
