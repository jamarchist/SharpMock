using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Syntax;
using TestUtilities;

namespace IntegrationTests.IntegrationTests
{
    [TestFixture]
    public class WhenFakingInterfaces : SharpMockTests
    {
        [Test]
        public void MethodCallIsIntercepted()
        {
            var wasCalled = false;

            Replace.CallsTo<ISomeInterface>(i => i.DoSomething()).With(() => wasCalled = true);

            var code = new CodeUnderTest();
            code.CallsSomeInterface(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
