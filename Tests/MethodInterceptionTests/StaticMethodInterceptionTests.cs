using System;
using System.Collections.Generic;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;
using Assert = NUnit.Framework.Assert;
using AssertAction = SharpMock.Core.Interception.InterceptionStrategies.Assert;

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
                new CompoundInterceptor(new AlwaysMatches(),
                    new InvokeCall(() => replacement)));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("This should not appear.", suppliedArgument);          
        }

        [Test]
        public void ReplacesConsoleWriteLineCall()
        {
            Action<string> replacement = s => Replacement.Call("Intercepted.");
            
            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new AlwaysMatches(),
                    new InvokeCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("Intercepted.", Replacement.ReplacementArg1);
        }

        [Test]
        public void InterceptsReturnValue()
        {
            ReplaceReturnValue.ReturnValueReplacementFunction replace = o => "Replacement.";
            
            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new AlwaysMatches(),
                    new InvokeOriginalCall(),
                    new ReplaceReturnValue(replace))
                );

            var mock = new CodeUnderTest();
            var returnValue = mock.CallsStringReturnNoParameters();

            Assert.AreEqual("Replacement.", returnValue);
        }

        [Test]
        public void InterceptsArguments()
        {
            Function<IList<object>, IList<object>> replace = args => new List<object> { 5555 };
            
            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new AlwaysMatches(),
                    new ReplaceArguments(() => replace),
                    new InvokeOriginalCall()
                    ));

            var mocked = new CodeUnderTest();
            var result = mocked.CallsStringReturnOneParameter();
        
            Assert.AreEqual("|| Original method return value when passed '5555'. ||", result);
        }

        [Test]
        public void InterceptsArgumentsAndReplacesMethod()
        {
            Function<int, string> replacementMethod = number => String.Format("Intercepted: {0}", number);
            Function<IList<object>, IList<object>> replaceArgs = args => new List<object>{ 4444 };

            var compoundInterceptor = new CompoundInterceptor(new AlwaysMatches(),
                    new ReplaceArguments(() =>replaceArgs),
                    new InvokeCall(() => replacementMethod)
                );

            InterceptorRegistry.AddInterceptor(compoundInterceptor);

            var mocked = new CodeUnderTest();
            var result = mocked.CallsStringReturnOneParameter();

            Assert.AreEqual("Intercepted: 4444", result);
        }

        [Test]
        public void InterceptsMethodAndAllowsCallingOriginal()
        {
            VoidAction<IInvocation> interceptor = i =>
            {
                var numberTimesTwo = (int)i.Arguments[0] * 2;
                var originalResult = i.OriginalCall.DynamicInvoke(numberTimesTwo);
                i.Return = String.Format("Intercepted: {0}", originalResult);
            };

            var compoundInterceptor = new CompoundInterceptor(
                new AlwaysMatches(),
                new InvokeWithInvocation(() => interceptor)
            );

            InterceptorRegistry.AddInterceptor(compoundInterceptor);

            var mocked = new CodeUnderTest();
            var result = mocked.CallsStringReturnOneParameter();

            Assert.AreEqual("Intercepted: || Original method return value when passed '1998'. ||", result);
        }
	}
}
