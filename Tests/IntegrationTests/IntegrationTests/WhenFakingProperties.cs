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
        public void InstanceSetterIsIntercepted()
        {
            Replace.CallsTo((SomeConcreteClass c) => c.SomeProperty = string.Empty).With(() => { });

            var code = new CodeUnderTest();
            code.CallsSomeConcreteClassPropertySetter();
        }

        [Test]
        public void InstanceGetterIsIntercepted()
        {
            Replace.CallsTo((SomeConcreteClass c) => c.SomeProperty).With(() => "some string");

            var code = new CodeUnderTest();
            var result = code.CallsSomeConcreteClassPropertyGetter();

            Assert.AreEqual("some string", result);
        }

        [Test]
        public void StaticGetterIsIntercepted()
        {
            Replace.CallsTo(() => StaticClass.StaticProperty).With(() => "some string");

            var code = new CodeUnderTest();
            var result = code.CallsStaticPropertyGetter();

            Assert.AreEqual("some string", result);
        }

        [Test]
        public void StaticSetterIsIntercepted()
        {
            Replace.CallsTo(() => StaticClass.StaticProperty = "ignored").With(() => { });

            var code = new CodeUnderTest();
            code.CallsStaticPropertySetter();
        }
    }
}