using System;
using System.Collections.Generic;
using NUnit.Framework;
using ScenarioDependencies;
using Scenarios;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.InterceptionStrategies;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.Interception.MatchingStrategies;
using SharpMock.Core.Interception.Registration;
using TestUtilities;
using Assert = NUnit.Framework.Assert;

namespace IntegrationTests.MethodInterceptionTests
{
	[TestFixture]
	public class WhenInterceptingStaticMethods : SharpMockTests
	{
        [Test]
        public void OriginalArgumentIsCaptured()
        {
            string suppliedArgument = null;
            Action<string> replacement = s => suppliedArgument = s;

            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new AlwaysMatches(),
                    new InsteadOfCall(() => replacement)));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("This should not appear.", suppliedArgument);          
        }

        [Test]
        public void CallIsReplaced()
        {
            Action<string> replacement = s => MethodReplacement.Call("Intercepted.");
            
            InterceptorRegistry.AddInterceptor(
                new CompoundInterceptor(new AlwaysMatches(),
                    new InsteadOfCall(() => replacement)
                ));

            var mocked = new CodeUnderTest();
            mocked.CallsConsoleWriteLine();

            Assert.AreEqual("Intercepted.", MethodReplacement.ReplacementArg1);
        }

        [Test]
        public void ReturnValueIsIntercepted()
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
        public void ArgumentsAreReplaced()
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
        public void ArgumentsAreReplacedAndCallIsReplaced()
        {
            Function<int, string> replacementMethod = number => String.Format("Intercepted: {0}", number);
            Function<IList<object>, IList<object>> replaceArgs = args => new List<object>{ 4444 };

            var compoundInterceptor = new CompoundInterceptor(new AlwaysMatches(),
                    new ReplaceArguments(() =>replaceArgs),
                    new InsteadOfCall(() => replacementMethod)
                );

            InterceptorRegistry.AddInterceptor(compoundInterceptor);

            var mocked = new CodeUnderTest();
            var result = mocked.CallsStringReturnOneParameter();

            Assert.AreEqual("Intercepted: 4444", result);
        }

        [Test]
        public void MethodIsReplacedAndOriginalIsCalled()
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

        [Test]
        public void LogicIsInsertedBefore()
        {
            VoidAction<IInvocation> interceptor = i => Console.WriteLine("BEFORE " + i.OriginalCall.Method.Name);

            var compoundInterceptor = 
                new CompoundInterceptor(new AlwaysMatches(), new BeforeCall(() => interceptor));
        
            InterceptorRegistry.AddInterceptor(compoundInterceptor);

            var mocked = new CodeUnderTest();
            var result = mocked.CallsStringReturnOneParameter();

            Assert.AreEqual("|| Original method return value when passed '999'. ||", result);
        }

        [Test]
        public void LogicIsInsertedAfter()
        {
            VoidAction<IInvocation> interceptor = i =>
            {
                Console.WriteLine("AFTER " + i.OriginalCall.Method.Name);
                Console.WriteLine("METHOD RETURNED: {0}", i.Return);
            };

            var compoundInterceptor =
                new CompoundInterceptor(new AlwaysMatches(), new AfterCall(() => interceptor));

            InterceptorRegistry.AddInterceptor(compoundInterceptor);

            var mocked = new CodeUnderTest();
            var result = mocked.CallsStringReturnOneParameter();

            Assert.AreEqual("|| Original method return value when passed '999'. ||", result);            
        }
	}

    public class WhenInterceptingStaticMethodsSpecification : IReplacementSpecification
    {
        public IList<ReplaceableMethodInfo> GetMethodsToReplace()
        {
            var list = new List<ReplaceableMethodInfo>();

            list.Add(SharpMock.Core.StaticReflection.VoidMethod.Of(Console.WriteLine).AsReplaceable());
            list.Add(SharpMock.Core.StaticReflection.Method.Of(StaticClass.StringReturnNoParameters).AsReplaceable());
            list.Add(SharpMock.Core.StaticReflection.Method.Of<int, string>(StaticClass.StringReturnOneParameter).AsReplaceable());

            return list;
        }
    }
}
