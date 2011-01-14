using System;

namespace ScenarioDependencies
{
	public static class StaticClass
	{
		public static void VoidReturnNoParameters()
		{
			throw new MethodNotInterceptedException(
				"StaticClass.VoidReturnNoParameters should have been intercepted, but was called instead.");
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
	}
}
