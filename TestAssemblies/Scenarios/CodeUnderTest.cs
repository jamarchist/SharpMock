﻿using System;
using System.Collections.Generic;
using System.Reflection;
using ScenarioDependencies;

namespace Scenarios
{
    public class SomethingWithUnusedBoolReturn
    {
        public bool ShouldIntercept(MethodInfo method)
        {
            return true;
        }
    }

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
        public static string WhatIWantThisToLookLikeForStaticMethods(string x, int y)
        {
            //Function<string, int, string> originalCall =
            //    (replacedX, replacedY) => InterceptedCall(replacedX, replacedY);

            var interceptedType = typeof (CodeUnderTest);
            var parameterTypes = new Type[2];
            parameterTypes[0] = typeof (string);
            parameterTypes[1] = typeof (int);
            var interceptedMethod = interceptedType.GetMethod("WhatIWantThisToLookLikeForInstanceMethods", parameterTypes);
            var interceptor = new SomethingWithUnusedBoolReturn();
            var notUsed = interceptor.ShouldIntercept(interceptedMethod);

            var arguments = new List<object>();
            arguments.Add(x);
            arguments.Add(y);

            if (x == "blah")
            {
                return null;
            }

            return x;

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
