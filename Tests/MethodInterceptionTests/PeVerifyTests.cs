using NUnit.Framework;

namespace MethodInterceptionTests
{
    [TestFixture]
    public class PeVerifyTests
    {
        [Test]
        public void TestAssemblyPassesPeVerification()
        {
            var result = PeVerify.VerifyAssembly(@"C:\Projects\github\SharpMock\Tests\MethodInterceptionTests\bin\Debug\MethodInterceptionTests.dll");
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.MetaDataErrors.Count);
        }

        [Test]
        public void TargetAssemblyPassesPeVerification()
        {
            var result = PeVerify.VerifyAssembly(@"C:\Projects\github\SharpMock\Tests\MethodInterceptionTests\bin\Debug\Scenarios.dll");
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Errors.Count);
        }
    }
}
