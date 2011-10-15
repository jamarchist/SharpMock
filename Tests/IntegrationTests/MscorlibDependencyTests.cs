using System;
using System.Reflection;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;

namespace IntegrationTests
{
    [TestFixture]
    public class MscorlibDependencyTests
    {
        [TearDown]
        public void ClearRegistry()
        {
            InterceptorRegistry.Clear();
        }

        [Test]
        public void InterceptsPrimitiveParsing()
        {
            var faker = new Faker();

            faker.CallsTo(() => bool.Parse("")).ByReplacingWith((string x) => false);
            faker.CallsTo(() => int.Parse("")).ByReplacingWith((string x) => 4);
            faker.CallsTo(() => decimal.Parse("")).ByReplacingWith((string x) => 3.14M);

            var code = new CodeWithMscorlibDependencies();
            var result = code.ParsesHardCodedStrings();

            Assert.AreEqual(false, result.FirstValue);
            Assert.AreEqual(4, result.SecondValue);
            Assert.AreEqual(3.14M, result.ThirdValue);
        }

        [Test]
        public void InterceptsCreateDelegate()
        {
            var fake = new Faker();
            var wasCalled = false;
            VoidAction setWasCalledEqualToTrue = () => { wasCalled = true; };
            Delegate substitute = setWasCalledEqualToTrue;

            fake.CallsTo(() => Delegate.CreateDelegate(null, null)).ByReplacingWith((Type t, MethodInfo m) => substitute);

            var code = new CodeWithMscorlibDependencies();
            var createdDelegate = code.CreatesDelegate();
            createdDelegate.DynamicInvoke(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void InterceptsEnvironmentProperty()
        {
            var fake = new Faker();

            fake.CallsTo(() => Environment.MachineName).ByReplacingWith(() => "Machine-X");

            var code = new CodeWithMscorlibDependencies();
            var machineName = code.GetsMachineName();

            Assert.AreEqual("Machine-X", machineName);
        }

        [Test]
        public void InterceptsDateTimeProperty()
        {
            var fake = new Faker();
            var october25th1985 = new DateTime(1985, 10, 25);

            fake.CallsTo(() => DateTime.Now).ByReplacingWith(() => october25th1985);

            var code = new CodeWithMscorlibDependencies();
            var result = code.GetsCurrentDateTime();

            Assert.AreEqual(october25th1985, result);
        }

        [Test]
        public void InterceptsFileSystemMethods()
        {
            var fake = new Faker();

            fake.CallsTo(() => System.IO.File.Exists(null)).ByReplacingWith((string s) => true);

            var code = new CodeWithMscorlibDependencies();
            var result = code.ChecksIfFileExists();

            Assert.IsTrue(result);
        }
    }
}
