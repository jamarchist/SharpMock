using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.Matchers;

namespace MethodInterceptionTests
{
	[TestFixture]
	public class StaticMethodInterceptionTests
	{
        public static class Replacement
        {
            public static object ReplacementArg1 { get; private set; }
            public static void Call(object replacementArg)
            {
                ReplacementArg1 = replacementArg;
            }
        }

        [TearDown]
        public void ClearRegistry()
        {
            InterceptorRegistry.Clear();
        }

        [Test]
        public void CapturesOriginalConsoleWriteLineArgument()
        {
            string suppliedArgument = null;
            Action<string> replacement = s => suppliedArgument = s;

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new ReplacementMethodInterceptor(replacement),
                    new InvokingInterceptor(new AlwaysMatchesMatcher())));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("This should not appear.", suppliedArgument);          
        }

        [Test]
        public void ReplacesConsoleWriteLineCall()
        {
            Action<string> replacement = s => Replacement.Call("Intercepted.");
            
            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new ReplacementMethodInterceptor(replacement),
                    new InvokingInterceptor(new AlwaysMatchesMatcher())
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("Intercepted.", Replacement.ReplacementArg1);
        }

        [Test]
        public void InterceptsReturnValue()
        {
            ReturnValueInterceptor.ReturnValueReplacementFunction replace = o => "Replacement.";
            
            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new InvokingInterceptor(new AlwaysMatchesMatcher()),
                    new ReturnValueInterceptor(replace))
                );

            var mock = new CodeUnderTest();
            var returnValue = mock.CallsStringReturnNoParameters();

            Assert.AreEqual("Replacement.", returnValue);
        }

        [Test]
        public void InterceptsArguments()
        {
            ArgumentsInterceptor.ArgumentValuesReplacementFunction replace = args => new List<object> { 5555 };
            
            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new ArgumentsInterceptor(replace),
                    new InvokingInterceptor(new AlwaysMatchesMatcher())
                    ));

            var mocked = new CodeUnderTest();
            var result = mocked.CallsStringReturnOneParameter();
        
            Assert.AreEqual("|| Original method return value when passed '5555'. ||", result);
        }

        [Test]
        public void InterceptsArgumentsAndReplacesMethod()
        {
            Function<int, string> replacementMethod = number => String.Format("Intercepted: {0}", number);
            ArgumentsInterceptor.ArgumentValuesReplacementFunction replaceArgs = args => new List<object>{ 4444 };

            var compoundInterceptor = new CompoundInterceptor(
                    new ArgumentsInterceptor(replaceArgs),
                    new ReplacementMethodInterceptor(replacementMethod),
                    new InvokingInterceptor(new AlwaysMatchesMatcher())
                );

            InterceptorRegistry.AddInterceptor(compoundInterceptor);

            var mocked = new CodeUnderTest();
            var result = mocked.CallsStringReturnOneParameter();

            Assert.AreEqual("Intercepted: 4444", result);
        }

        [Test]
        public void InterceptsWhenMethodCallsMatchExactly()
        {
            Action<string> replacement = s => Replacement.Call("Intercepted.");
            var console = typeof (Console);
            var writeLine = console.GetMethod("WriteLine", new[] {typeof (string)});

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new ReplacementMethodInterceptor(replacement),
                    new InvokingInterceptor(new EquivalentCallMatcher(writeLine))
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("Intercepted.", Replacement.ReplacementArg1);
        }

        [Test]
        public void DoesNotInterceptWhenMethodCallsDoNotMatchExactly()
        {
            Action<string> replacement = s => Replacement.Call("Intercepted.");
            var console = typeof(Console);
            var writeLineWithFormatString = console.GetMethod("WriteLine", new[] { typeof(string), typeof(string) });

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new ReplacementMethodInterceptor(replacement),
                    new InvokingInterceptor(new EquivalentCallMatcher(writeLineWithFormatString))
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreNotEqual("Intercepted.", Replacement.ReplacementArg1);            
        }
	}
}
