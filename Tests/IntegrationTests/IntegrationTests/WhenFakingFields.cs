using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Syntax;

namespace IntegrationTests.IntegrationTests
{
    [TestFixture]
    public class WhenFakingFields
    {
        [Test]
        public void FieldAccessIsFaked()
        {
            Replace.CallsTo(() => StaticClass.StaticField).With(() => 99);

            var code = new CodeUnderTest();
            var result = code.CallsStaticField();

            Assert.AreEqual(99, result);
        }

        [Test]
        public void FieldAssignmentIsFaked()
        {
            Assert.Fail("Field assignment faking is not implemented.");
        }
    }
}
