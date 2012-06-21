using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class AllOverloadsMatch : IMatchingStrategy
    {
        private readonly Type declaringType;
        private readonly string methodName;

        public AllOverloadsMatch(MethodInfo method)
        {
            declaringType = method.DeclaringType;
            methodName = method.Name;
        }

        public bool Matches(MethodInfo calledMethod, IList<object> arguments)
        {
            if (calledMethod.DeclaringType.Equals(declaringType) && calledMethod.Name == methodName)
            {
                return true;
            }

            return false;
        }
    }
}
