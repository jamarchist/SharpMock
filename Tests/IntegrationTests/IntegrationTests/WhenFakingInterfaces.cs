using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Syntax;

namespace IntegrationTests.IntegrationTests
{
    [TestFixture]
    public class WhenFakingInterfaces : SharpMockTests
    {
        [Test]
        public void MethodCallIsIntercepted()
        {
            var wasCalled = false;

            var fake = new Faker();
            fake.CallsTo<ISomeInterface>(i => i.DoSomething()).ByReplacingWith(() => wasCalled = true);

            var code = new CodeUnderTest();
            code.CallsSomeInterface(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
