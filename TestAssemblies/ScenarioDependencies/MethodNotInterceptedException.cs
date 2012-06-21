using System;

namespace ScenarioDependencies
{
	public class MethodNotInterceptedException : ApplicationException
	{
		public MethodNotInterceptedException(string methodName) : 
            base(String.Format("{0} should have been intercepted but was not.", methodName))
		{
			
		}
	}
}
