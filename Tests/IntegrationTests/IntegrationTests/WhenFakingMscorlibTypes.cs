using System;
using System.Reflection;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core;
using SharpMock.Core.Syntax;
using TestUtilities;

namespace IntegrationTests.IntegrationTests
{
    //[TestFixture]
    //public class WhenFakingMscorlibTypes : SharpMockTests
    //{
    //    [Test]
    //    public void MethodsOnPrimitivesAreIntercepted()
    //    {
    //        Replace.CallsTo(() => bool.Parse("")).With((string x) => false);
    //        Replace.CallsTo(() => int.Parse("")).With((string x) => 4);
    //        Replace.CallsTo(() => decimal.Parse("")).With((string x) => 3.14M);

    //        var code = new CodeWithMscorlibDependencies();
    //        var result = code.ParsesHardCodedStrings();

    //        Assert.AreEqual(false, result.FirstValue);
    //        Assert.AreEqual(4, result.SecondValue);
    //        Assert.AreEqual(3.14M, result.ThirdValue);
    //    }

    //    [Test]
    //    public void CreateDelegateMethodIsIntercepted()
    //    {
    //        var wasCalled = false;
    //        VoidAction setWasCalledEqualToTrue = () => { wasCalled = true; };
    //        Delegate substitute = setWasCalledEqualToTrue;

    //        Replace.CallsTo(() => Delegate.CreateDelegate(null, null)).With((Type t, MethodInfo m) => substitute);

    //        var code = new CodeWithMscorlibDependencies();
    //        var createdDelegate = code.CreatesDelegate();
    //        createdDelegate.DynamicInvoke(null);

    //        Assert.IsTrue(wasCalled);
    //    }

    //    [Test]
    //    public void PropertyGetterIsIntercepted()
    //    {
    //        Replace.CallsTo(() => Environment.MachineName).With(() => "Machine-X");

    //        var code = new CodeWithMscorlibDependencies();
    //        var machineName = code.GetsMachineName();

    //        Assert.AreEqual("Machine-X", machineName);
    //    }

    //    [Test]
    //    public void DateTimePropertyIsIntercepted()
    //    {
    //        var october25th1985 = new DateTime(1985, 10, 25);

    //        Replace.CallsTo(() => DateTime.Now).With(() => october25th1985);

    //        var code = new CodeWithMscorlibDependencies();
    //        var result = code.GetsCurrentDateTime();

    //        Assert.AreEqual(october25th1985, result);
    //    }

    //    [Test]
    //    public void FileSystemMethodIsIntercepted()
    //    {
    //        Replace.CallsTo(() => System.IO.File.Exists(null)).With((string s) => true);

    //        var code = new CodeWithMscorlibDependencies();
    //        var result = code.ChecksIfFileExists();

    //        Assert.IsTrue(result);
    //    }
    //}
}
