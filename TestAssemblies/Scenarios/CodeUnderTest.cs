using System;
using System.Collections.Generic;
using ScenarioDependencies;
using SharpMock.Core;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;

namespace Scenarios
{
	public class CodeUnderTest
	{
        public string CallsStringReturnNoParameters()
        {
            return StaticClass.StringReturnNoParameters();
        }

        public string CallsStringReturnOneParameter()
        {
            return StaticClass.StringReturnOneParameter(999);
        }

        public void CallsConsoleWriteLine()
        {
            Console.WriteLine("This should not appear.");
        }

        public void CallsConsoleWriteLineNotIntercepted()
        {
            Console.WriteLine("This *should* appear.");
        }

        public void CallsConsoleWriteLineFormatStingOverload()
        {
            Console.WriteLine("{0}-{0} should not appear.", 1, "This");
        }

        public void CallsVoidReturnNoParameters()
        {
            StaticClass.VoidReturnNoParameters();
        }

        public Tuple<string, string> CallsTwoMethods()
        {
            var firstResult = StaticClass.StringReturnNoParameters();
            var secondResult = StaticClass.StringReturnOneParameter(9876);

            return new Tuple<string, string>(firstResult, secondResult);
        }

        public void CallsSealedMethod()
        {
            var s = new SealedClass();
            s.VoidReturnNoParameters();
        }

        public string CallsSealedMethodWithParameter(int parameterValue)
        {
            var s = new SealedClass();
            return s.StringReturnOneParameter(parameterValue);
        }

        public void CallsSomeConcreteClassMethod(SomeConcreteClass someInstance)
        {
            someInstance.SomeMethod();
        }

        public void CallsSomeInterface(ISomeInterface someInterface)
        {
            someInterface.DoSomething();
        }

        public SomeConcreteClass CallsConstructor()
        {
            var x = new SomeConcreteClass();
            return x;
        }

        #region MoveToSampleExaminerProject
        //public object GetsCalled(string x, int y, object z)
        //{
        //    return null;
        //}

        //public object DoesCalling(string x, int y, object z)
        //{
        //    return GetsCalled(x, y, z);
        //}

        //public void DeclaresAnonymousMethod(string sParam)
        //{
        //    Function<string, int> anon = x => Convert.ToInt32(sParam);

        //    //Function<string> anon2 = () => sParam.Substring(1);

        //    var return1 = anon(sParam);
        //    //var return2 = anon2();

        //    //Console.WriteLine(return1);
        //    //Console.WriteLine(return2);
        //}

        //public object WhatIWantThisToLookLike(string x, int y)
        //{
        //    Function<string, int, object> originalCall =
        //        (replacedX, replacedY) => InterceptedCall(replacedX, replacedY);

        //    var arguments = new List<object>();
        //    arguments.Add(x);
        //    arguments.Add(y);

        //    var invocation = new Invocation();
        //    invocation.Arguments = arguments;
        //    invocation.OriginalCall = originalCall;

        //    var interceptor = new RegistryInterceptor();
        //    interceptor.Intercept(invocation);

        //    return invocation.Return;
        //}

        //public static string InterceptedCall(string x, int y)
        //{
        //    return null;
        //}

        //public static object CallsProperty()
        //{
        //    var sample = new Sample();
        //    return sample.Name;
        //}

        //// For instance methods
        public static void VoidReturnNoParameters(SealedClass c)
        {
            var interceptor = new RegistryInterceptor();
            var invocation = new Invocation();
            
            var interceptedType = typeof(SealedClass);
            var parameterTypes = new Type[0];
            //parameterTypes[0] = typeof(string);

            var interceptedMethod = interceptedType.GetMethod("VoidReturnNoParameters", parameterTypes);
            VoidAction<SealedClass> originalCall = (SealedClass s) => s.VoidReturnNoParameters();

            var arguments = new List<object>();
            //arguments.Add(x);
            
            invocation.OriginalCall = originalCall;
            invocation.Arguments = arguments;
            invocation.Target = c;

            var notUsed = interceptor.ShouldIntercept(interceptedMethod, arguments);
            interceptor.Intercept(invocation);

            //return (decimal)invocation.Return;

            //var interceptor = new RegistryInterceptor();
            //if (interceptor.ShouldIntercept(interceptedMethod))
            //{
            //    var invocation = new Invocation();
            //    invocation.Arguments = arguments;
            //    invocation.OriginalCall = originalCall;
            //    invocation.Target = null;

            //    interceptor.Intercept(invocation);

            //    return (string)invocation.Return;                
            //}

            //return originalCall(x, y);

            //return String.Empty;
        }

        //public class Sample
        //{
        //    public string Name { get; set; }

        //    public object InterceptedCall(string x, int y)
        //    {
        //        return null;
        //    }
        //}
        #endregion
	}
}
