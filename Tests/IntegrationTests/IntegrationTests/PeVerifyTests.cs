using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace IntegrationTests
{
    [TestFixture]
    public class PeVerifyTests
    {
        [Test]
        public void TestAssemblyPassesPeVerification()
        {
            var result = PeVerify.VerifyAssembly(@"C:\Projects\github\SharpMock\Tests\IntegrationTests\bin\Debug\IntegrationTests.dll", true);
            Assert.AreEqual(0, result.Errors.Count, PrintErrors(result.Errors));
            Assert.AreEqual(0, result.MetaDataErrors.Count, PrintErrors(result.MetaDataErrors));
        }

        [Test]
        public void TargetAssemblyPassesPeVerification()
        {
            var result = PeVerify.VerifyAssembly(@"C:\Projects\github\SharpMock\Tests\IntegrationTests\bin\Debug\Scenarios.dll", true);
            Assert.AreEqual(0, result.Errors.Count, PrintErrors(result.Errors));
            Assert.AreEqual(0, result.MetaDataErrors.Count, PrintErrors(result.MetaDataErrors));
        }

        private string PrintErrors(IEnumerable<string> errors)
        {
            var errorMessage = new StringBuilder();

            errorMessage.Append("PeVerify failed with the following errors:");
            errorMessage.Append(Environment.NewLine);
            foreach (var error in errors)
            {
                errorMessage.Append(error);
                errorMessage.Append(Environment.NewLine);
            }

            return errorMessage.ToString();
        }
    }
}
