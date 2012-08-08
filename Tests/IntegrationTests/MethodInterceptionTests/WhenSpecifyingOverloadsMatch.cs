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
    public class WhenSpecifyingOverloadsMatch : SharpMockTests
    {
        //[Test]
        //public void MethodWithMatchingOverloadIsIntercepted()
        //{
        //    var dummy = new MethodReplacement();
        //    Action<string> replacement = s => dummy.Call("Intercepted.");

        //    InterceptorRegistry.AddInterceptor(
        //        new CompoundInterceptor(new AllOverloadsMatch(VoidMethod.Of(Console.WriteLine)),
        //            new InvokeCallSafe(() => replacement)
        //        ));

        //    var mocked = new CodeUnderTest();
        //    mocked.CallsConsoleWriteLineFormatStingOverload();

        //    Assert.AreEqual("Intercepted.", dummy.ReplacementArg1);            
        //}
    }
}
