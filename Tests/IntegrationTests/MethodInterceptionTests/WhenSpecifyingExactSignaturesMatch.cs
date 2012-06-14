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

namespace IntegrationTests.MethodInterceptionTests
{
    [TestFixture]
    public class WhenSpecifyingExactSignaturesMatch : SharpMockTests
    {
        [Test]
        public void ExactMatchesAreIntercepted()
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
        public void DifferingSignaturesAreNotIntercepted()
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
    }
}