using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler;
using SharpMock.PostCompiler.Core;
using Assert = NUnit.Framework.Assert;

namespace MethodInterceptionTests
{
    [TestFixture]
    public class RegisteredInterceptionTests
    {
        [Test]
        public void CanInterceptSpecifiedMethods()
        {
            var spec = new TestSpecification();
            spec.SpecifyInterceptors(new SpecificationRegistry());

            var mocked = new CodeUnderTest();
            var result = mocked.CallsStringReturnOneParameter();

            Assert.AreEqual("|| Original method return value when passed '888'. ||", result);
        }

        [TearDown]
        public void ClearRegistry()
        {
            InterceptorRegistry.Clear();
        }
    }
}
