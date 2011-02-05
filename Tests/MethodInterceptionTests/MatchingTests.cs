using System;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.Matchers;
using SharpMock.Core.Interception.MatchingStrategies;

namespace MethodInterceptionTests
{
    [TestFixture]
    public class MatchingTests
    {
        [Test]
        public void InterceptsWhenMethodCallsMatchExactly()
        {
            Action<string> replacement = s => StaticMethodInterceptionTests.Replacement.Call("Intercepted.");
            var console = typeof(Console);
            var writeLine = console.GetMethod("WriteLine", new[] { typeof(string) });

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new EquivalentCallsMatch(writeLine),
                    new ReplaceCall(replacement),
                    new InvokeCall()
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("Intercepted.", StaticMethodInterceptionTests.Replacement.ReplacementArg1);
        }

        [Test]
        public void DoesNotInterceptWhenMethodCallsDoNotMatchExactly()
        {
            Action<string> replacement = s => StaticMethodInterceptionTests.Replacement.Call("Intercepted.");
            var console = typeof(Console);
            var writeLineWithFormatString = console.GetMethod("WriteLine", new[] { typeof(string), typeof(string) });

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new EquivalentCallsMatch(writeLineWithFormatString),
                    new ReplaceCall(replacement),
                    new InvokeCall()
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreNotEqual("Intercepted.", StaticMethodInterceptionTests.Replacement.ReplacementArg1);
        }

        [Test]
        public void InterceptsMethodWithMatchingOverload()
        {
            Action<string> replacement = s => StaticMethodInterceptionTests.Replacement.Call("Intercepted.");
            var console = typeof(Console);

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new AllOverloadsMatch(console, "WriteLine"),
                    new ReplaceCall(replacement),
                    new InvokeCallSafe()
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLineFormatStingOverload();

            Assert.AreEqual("Intercepted.", StaticMethodInterceptionTests.Replacement.ReplacementArg1);            
        }
    }
}
