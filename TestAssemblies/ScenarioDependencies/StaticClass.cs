using System;

namespace ScenarioDependencies
{
	public static class StaticClass
	{
	    public static string StaticProperty
	    {
	        get
	        {
	            throw new MethodNotInterceptedException("StaticClass.get_StaticProperty");
	        }
            set
            {
                throw new MethodNotInterceptedException("StaticClass.set_StaticProperty");
            }
	    }

		public static void VoidReturnNoParameters()
		{
			throw new MethodNotInterceptedException("StaticClass.VoidReturnNoParameters");
		}

        public static void VoidReturnNoParameters(string ignoredOverloadParameter)
        {
            throw new MethodNotInterceptedException("StaticClass.VoidReturnNoParameters(string)");
        }

        public static void VoidReturnNoParameters(char[] ignoredOverloadParameterArray)
        {
            throw new MethodNotInterceptedException("StaticClass.VoidReturnNoParameters(char[])");
        }

        public static void VoidReturnNoParameters(string ignoredOverloadString, params object[] ignoredParams)
        {
            throw new MethodNotInterceptedException("StaticClass.VoidReturnNoParameters(string, object[])");
        }

        public static string StringReturnNoParameters()
        {
            return "|| Original method return value. ||";
            //throw new MethodNotInterceptedException(
            //    "StaticClass.StringReturnNoParameters should have been intercepted, but was called instead.");
        }

        public static string StringReturnOneParameter(int param1)
        {
            return String.Format("|| Original method return value when passed '{0}'. ||", param1);
            //throw new MethodNotInterceptedException(
            //    "StaticClass.StringReturnOneParameter should have been intercepted, but was called instead.");
        }

        public static void Overloaded()
        {
            throw new MethodNotInterceptedException("StaticClass.Overload()");
        }

        public static void Overloaded(string ignored)
        {
            throw new MethodNotInterceptedException("StaticClass.Overloaded(string)");
        }

	    public static int StaticField;
	}
}
