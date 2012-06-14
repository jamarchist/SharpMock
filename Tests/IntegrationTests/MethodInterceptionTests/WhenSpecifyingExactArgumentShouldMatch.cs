using System;
using NUnit.Framework;
using Scenarios;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;
using TestUtilities;
using Assert = NUnit.Framework.Assert;

namespace IntegrationTests.MethodInterceptionTests
{
    [TestFixture]
    public class WhenSpecifyingExactArgumentShouldMatch : SharpMockTests
    {
        [Test]
        public void MethodCallWithMatchingArgumentsIsIntercepted()
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
        public void MethodCallWithNonMatchingArgumentsIsNotIntercepted()
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