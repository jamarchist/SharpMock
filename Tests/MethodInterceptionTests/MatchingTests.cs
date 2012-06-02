using System;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;
using SharpMock.Core.StaticReflection;
using TestUtilities;
using Assert = NUnit.Framework.Assert;

namespace MethodInterceptionTests
{
    [TestFixture]
    public class MatchingTests
    {
        [SetUp]
        public void ClearRegistryFirst()
        {
            ClearRegistry();
        }

        [TearDown]
        public void ClearRegistry()
        {
            InterceptorRegistry.Clear();
        }

        [Test]
        public void InterceptsWhenMethodCallsMatchExactly()
        {
            Action<string> replacement = s => MethodReplacement.Call("Intercepted.");
            var console = typeof(Console);
            var writeLine = console.GetMethod("WriteLine", new[] { typeof(string) });

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new EquivalentCallsMatch(writeLine),
                    new InsteadOfCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("Intercepted.", MethodReplacement.ReplacementArg1);
        }

        [Test]
        public void DoesNotInterceptWhenMethodCallsDoNotMatchExactly()
        {
            Action<string> replacement = s => MethodReplacement.Call("Intercepted.");
            var writeLineWithFormatString = VoidMethod.Of<string, string>(Console.WriteLine);

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new EquivalentCallsMatch(writeLineWithFormatString),
                    new InsteadOfCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLineNotIntercepted();

            Assert.AreNotEqual("Intercepted.", MethodReplacement.ReplacementArg1);
        }

        [Test]
        public void InterceptsMethodWithMatchingOverload()
        {
            Action<string> replacement = s => MethodReplacement.Call("Intercepted.");

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new AllOverloadsMatch(VoidMethod.Of(Console.WriteLine)),
                    new InvokeCallSafe(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLineFormatStingOverload();

            Assert.AreEqual("Intercepted.", MethodReplacement.ReplacementArg1);            
        }

        [Test]
        public void InterceptsMethodWithMatchingArguments()
        {
            var arg = "This should not appear.";
            Action<string> replacement = s => MethodReplacement.Call("Intercepted.");
            var console = typeof(Console);
            var writeLine = console.GetMethod("WriteLine", new[] {typeof (string)});

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new ArgumentsMatch(new EquivalentCallsMatch(writeLine), new MatchesExactly(arg)),
                    new InsteadOfCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("Intercepted.", MethodReplacement.ReplacementArg1);             
        }

        [Test]
        public void DoesNotIntercepMethodWithNonMatchingArguments()
        {
            Action<string> replacement = s => MethodReplacement.Call("Intercepted.");
            var console = typeof(Console);
            var writeLine = console.GetMethod("WriteLine", new[] { typeof(string) });

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(
                    new ArgumentsMatch(new EquivalentCallsMatch(writeLine), new MatchesExactly("Something that doesn't match.")),
                    new InsteadOfCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLineNotIntercepted();

            Assert.AreNotEqual("Intercepted.", MethodReplacement.ReplacementArg1);
        }
    }
}
