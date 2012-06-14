using System;
using System.Reflection;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core;
using SharpMock.Core.Syntax;
using TestUtilities;

namespace IntegrationTests.IntegrationTests
{
    [TestFixture]
    public class WhenFakingMscorlibTypes : SharpMockTests
    {
        [Test]
        public void MethodsOnPrimitivesAreIntercepted()
        {
            var fake = new Faker();

            fake.CallsTo(() => bool.Parse("")).ByReplacingWith((string x) => false);
            fake.CallsTo(() => int.Parse("")).ByReplacingWith((string x) => 4);
            fake.CallsTo(() => decimal.Parse("")).ByReplacingWith((string x) => 3.14M);

            var code = new CodeWithMscorlibDependencies();
            var result = code.ParsesHardCodedStrings();

            Assert.AreEqual(false, result.FirstValue);
            Assert.AreEqual(4, result.SecondValue);
            Assert.AreEqual(3.14M, result.ThirdValue);
        }

        [Test]
        public void CreateDelegateMethodIsIntercepted()
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
        public void PropertyGetterIsIntercepted()
        {
            var fake = new Faker();

            fake.CallsTo(() => Environment.MachineName).ByReplacingWith(() => "Machine-X");

            var code = new CodeWithMscorlibDependencies();
            var machineName = code.GetsMachineName();

            Assert.AreEqual("Machine-X", machineName);
        }

        [Test]
        public void DateTimePropertyIsIntercepted()
        {
            var fake = new Faker();
            var october25th1985 = new DateTime(1985, 10, 25);

            fake.CallsTo(() => DateTime.Now).ByReplacingWith(() => october25th1985);

            var code = new CodeWithMscorlibDependencies();
            var result = code.GetsCurrentDateTime();

            Assert.AreEqual(october25th1985, result);
        }

        [Test]
        public void FileSystemMethodIsIntercepted()
        {
            var fake = new Faker();

            fake.CallsTo(() => System.IO.File.Exists(null)).ByReplacingWith((string s) => true);

            var code = new CodeWithMscorlibDependencies();
            var result = code.ChecksIfFileExists();

            Assert.IsTrue(result);
        }
    }
}
