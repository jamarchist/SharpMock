using System;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;

namespace MethodInterceptionTests
{
    [TestFixture]
    public class MatchingTests
    {
        [TearDown]
        public void ClearRegistry()
        {
            InterceptorRegistry.Clear();
        }

        [Test]
        public void InterceptsWhenMethodCallsMatchExactly()
        {
            Action<string> replacement = s => StaticMethodInterceptionTests.Replacement.Call("Intercepted.");
            var console = typeof(Console);
            var writeLine = console.GetMethod("WriteLine", new[] { typeof(string) });

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new EquivalentCallsMatch(writeLine),
                    new InvokeCall(() => replacement)
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
                    new InvokeCall(() => replacement)
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
                    new InvokeCallSafe(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLineFormatStingOverload();

            Assert.AreEqual("Intercepted.", StaticMethodInterceptionTests.Replacement.ReplacementArg1);            
        }

        [Test]
        public void InterceptsMethodWithMatchingArguments()
        {
            var arg = "This should not appear.";
            Action<string> replacement = s => StaticMethodInterceptionTests.Replacement.Call("Intercepted.");
            var console = typeof(Console);
            var writeLine = console.GetMethod("WriteLine", new[] {typeof (string)});

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new ArgumentsMatch(new EquivalentCallsMatch(writeLine), new MatchesExactly(arg)),
                    new InvokeCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("Intercepted.", StaticMethodInterceptionTests.Replacement.ReplacementArg1);             
        }

        [Test]
        public void DoesNotIntercepMethodWithNonMatchingArguments()
        {
            Action<string> replacement = s => StaticMethodInterceptionTests.Replacement.Call("Intercepted.");
            var console = typeof(Console);
            var writeLine = console.GetMethod("WriteLine", new[] { typeof(string) });

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new ArgumentsMatch(new EquivalentCallsMatch(writeLine), new MatchesExactly("Something that doesn't match.")),
                    new InvokeCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreNotEqual("Intercepted.", StaticMethodInterceptionTests.Replacement.ReplacementArg1);
        }
    }
}
