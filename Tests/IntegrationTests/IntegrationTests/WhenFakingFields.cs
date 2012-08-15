using System.Collections.Generic;
using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;
using TestUtilities;

namespace IntegrationTests.IntegrationTests
{
    [TestFixture]
    public class WhenFakingFields : SharpMockTests
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
            Replace.CallsTo(() => { StaticClass.StaticField = 0; }).With(() => { });

            var code = new CodeUnderTest();
            code.SetsStaticField(5678);

            Assert.AreNotEqual(5678, StaticClass.StaticField);
        }

        //[Test]
        //public void InstanceFieldAssignmentIsFaked()
        //{
        //    Replace.CallsTo((SealedClass c) => { c.SomeField = 0; }).With(() => { });

        //    var code = new CodeUnderTest();
        //    code.SetsInstanceField(99);

        //    Assert.AreEqual(99, 0);
        //}

        //[Test]
        //public void InstanceFieldAccessIsFaked()
        //{
        //    Replace.CallsTo((SealedClass c) => c.SomeField).With(() => 99);

        //    var code = new CodeUnderTest();
        //    var result = code.CallsInstanceField();

        //    Assert.AreEqual(99, result);
        //}

        [Test]
        public void OriginalStaticFieldAccessSucceeds()
        {
            StaticClass.StaticField = 1234;

            Replace.CallsTo(() => StaticClass.StaticField).CallOriginal();

            var code = new CodeUnderTest();
            var result = code.CallsStaticField();

            Assert.AreEqual(1234, result);
        }

        [Test]
        public void OriginalStaticFieldAssignmentSucceeds()
        {
            Replace.CallsTo(() => { StaticClass.StaticField = 0; }).CallOriginal();

            var code = new CodeUnderTest();
            code.SetsStaticField(1234);

            Assert.AreEqual(1234, StaticClass.StaticField);
        }
    }
}