using System;
using System.Reflection;
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
                    new InsteadOfCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("Intercepted.", StaticMethodInterceptionTests.Replacement.ReplacementArg1);
        }

        [Test]
        public void DoesNotInterceptWhenMethodCallsDoNotMatchExactly()
        {
            Action<string> replacement = s => StaticMethodInterceptionTests.Replacement.Call("Intercepted.");
            var writeLineWithFormatString = MethodOf<string, string>(Console.WriteLine);

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new EquivalentCallsMatch(writeLineWithFormatString),
                    new InsteadOfCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLineNotIntercepted();

            Assert.AreNotEqual("Intercepted.", StaticMethodInterceptionTests.Replacement.ReplacementArg1);
        }

        [Test]
        public void InterceptsMethodWithMatchingOverload()
        {
            Action<string> replacement = s => StaticMethodInterceptionTests.Replacement.Call("Intercepted.");

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new AllOverloadsMatch(MethodOf(Console.WriteLine)),
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
                    new InsteadOfCall(() => replacement)
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
                    new InsteadOfCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLineNotIntercepted();

            Assert.AreNotEqual("Intercepted.", StaticMethodInterceptionTests.Replacement.ReplacementArg1);
        }

        private MethodInfo MethodOf<T1, T2>(VoidAction<T1, T2> methodCall)
        {
            return methodCall.Method;
        }

        private MethodInfo MethodOf(VoidAction methodCall)
        {
            return methodCall.Method;
        }
    }
}
