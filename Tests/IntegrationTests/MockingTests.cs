﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Syntax;

namespace IntegrationTests
{
    //[TestFixture]
    //public class MockingTests
    //{
    //    [TearDown]
    //    public void ClearRegistry()
    //    {
    //        InterceptorRegistry.Clear();
    //    }

    //    [Test]
    //    public void CallsReplacementAction()
    //    {
    //        var wasCalled = false;

    //        var fake = new Faker();
    //        fake.CallsTo(() => StaticClass.VoidReturnNoParameters()).ByReplacingWith(() => wasCalled = true);

    //        var code = new CodeUnderTest();
    //        code.CallsVoidReturnNoParameters();

    //        Assert.IsTrue(wasCalled);
    //    }

    //    [Test]
    //    public void CallsReplacementFunction()
    //    {
    //        var fake = new Faker();
    //        fake.CallsTo(() => StaticClass.StringReturnNoParameters()).ByReplacingWith(() => "Interception Result");

    //        var code = new CodeUnderTest();
    //        var result = code.CallsStringReturnNoParameters();

    //        Assert.AreEqual("Interception Result", result);
    //    }

    //    [Test, ExpectedException(typeof(AssertionFailedException))]
    //    public void ThrowsAssertionException()
    //    {
    //        var fake = new Faker();
    //        fake.CallsTo(() => StaticClass.StringReturnOneParameter(1))
    //            .Asserting((int x) => x == 77)
    //            .ByReplacingWith((int x) => x.ToString());

    //        var code = new CodeUnderTest();
    //        code.CallsStringReturnOneParameter();
    //    }

    //    [Test]
    //    public void DoesNotThrowAssertionException()
    //    {
    //        var fake = new Faker();
    //        fake.CallsTo(() => StaticClass.StringReturnOneParameter(1))
    //            .ByReplacingWith((int x) => x.ToString())
    //            .Asserting((int x) => x == 999);

    //        var code = new CodeUnderTest();
    //        code.CallsStringReturnOneParameter();
    //    }

    //    [Test, ExpectedException(
    //        ExpectedException = typeof(InvalidOperationException), 
    //        ExpectedMessage = "I threw this from a replacement.")]
    //    public void ThrowExceptionFromReplacement()
    //    {
    //        var fake = new Faker();
    //        fake.CallsTo(() => StaticClass.VoidReturnNoParameters())
    //            .ByReplacingWith(() => { throw new InvalidOperationException("I threw this from a replacement."); });

    //        var code = new CodeUnderTest();
    //        code.CallsVoidReturnNoParameters();
    //    }

    //    [Test, ExpectedException(typeof(AssertionFailedException))]
    //    public void MakesMultipleAssertionsOrderOne()
    //    {
    //        var fake = new Faker();
    //        fake.CallsTo(() => StaticClass.StringReturnOneParameter(1))
    //            .Asserting((int x) => x > 10)
    //            .Asserting((int x) => x > 1000)
    //            .ByReplacingWith((int x) => "dummy return value");

    //        var code = new CodeUnderTest();
    //        code.CallsStringReturnOneParameter();
    //    }

    //    [Test, ExpectedException(typeof(AssertionFailedException))]
    //    public void MakesMultipleAssertionsOrderTwo()
    //    {
    //        var fake = new Faker();
    //        fake.CallsTo(() => StaticClass.StringReturnOneParameter(1))
    //            .Asserting((int x) => x > 1000)
    //            .Asserting((int x) => x > 10)
    //            .ByReplacingWith((int x) => "dummy return value");

    //        var code = new CodeUnderTest();
    //        code.CallsStringReturnOneParameter();
    //    }
    //}
}
