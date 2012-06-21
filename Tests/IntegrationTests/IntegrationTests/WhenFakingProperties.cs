using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Syntax;
using TestUtilities;

namespace IntegrationTests.IntegrationTests
{
    [TestFixture]
    public class WhenFakingProperties : SharpMockTests
    {
        [Test]
        public void SetterIsIntercepted()
        {
            Replace.CallsTo((SomeConcreteClass c) => c.SomeProperty = string.Empty).With(() => { });

            var code = new CodeUnderTest();
            code.CallsSomeConcreteClassPropertySetter();
        }
    }
}