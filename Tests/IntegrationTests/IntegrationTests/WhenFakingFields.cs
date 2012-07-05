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
        public void StaticFieldAccessIsFaked()
        {
            Replace.CallsTo(() => StaticClass.StaticField).With(() => 99);

            var code = new CodeUnderTest();
            var result = code.CallsStaticField();

            Assert.AreEqual(99, result);
        }

        [Test]
        public void StaticFieldAssignmentIsFaked()
        {
            Replace.CallsTo(() => StaticClass.StaticField = 0).With(() => { });

            var code = new CodeUnderTest();
            code.SetsStaticField(5678);

            Assert.AreNotEqual(5678, StaticClass.StaticField);
        }

        //[Test]
        //public void InstanceFieldAccessIsFaked()
        //{
        //    Replace.CallsTo((SealedClass s) => s.SomeField).With(() => 999);

        //    var code = new CodeUnderTest();
        //    var result = code.CallsInstanceField();
        
        //    Assert.AreEqual(999, result);
        //}
    }
}
