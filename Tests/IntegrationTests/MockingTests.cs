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
        public void ProvidesReplacementMethodAndCallsIt()
        {
            var wasCalled = false;

            var fake = new Faker();
            fake.CallsTo(() => StaticClass.StringReturnNoParameters()).ByReplacingWith(() => wasCalled = true);

            var code = new CodeUnderTest();
            code.CallsStringReturnNoParameters();

            Assert.IsTrue(wasCalled);
        }
    }
}
