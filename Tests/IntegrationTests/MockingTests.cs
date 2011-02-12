using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;

namespace IntegrationTests
{
    [TestFixture]
    public class MockingTests
    {
        [TearDown]
        public void ClearRegistry()
        {
            InterceptorRegistry.Clear();
        }

        [Test]
        public void CallsReplacementAction()
        {
            var wasCalled = false;

            var fake = new Faker();
            fake.CallsTo(() => StaticClass.VoidReturnNoParameters()).ByReplacingWith(() => wasCalled = true);

            var code = new CodeUnderTest();
            code.CallsVoidReturnNoParameters();

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void CallsReplacementFunction()
        {
            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnNoParameters()).ByReplacingWith(() => "Interception Result");

            var code = new CodeUnderTest();
            var result = code.CallsStringReturnNoParameters();

            Assert.AreEqual("Interception Result", result);
        }

        
    }
}
