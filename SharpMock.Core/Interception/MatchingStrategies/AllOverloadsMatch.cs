using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class AllOverloadsMatch : IMatchingStrategy
    {
        private readonly Type declaringType;
        private readonly string methodName;

        public AllOverloadsMatch(Type declaringType, string methodName)
        {
            this.declaringType = declaringType;
            this.methodName = methodName;
        }

        public bool Matches(MethodInfo calledMethod)
        {
            if (calledMethod.DeclaringType.Equals(declaringType) && calledMethod.Name == methodName)
            {
                return true;
            }

            return false;
        }
    }
}
