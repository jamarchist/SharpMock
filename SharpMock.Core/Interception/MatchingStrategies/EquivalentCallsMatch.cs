using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class EquivalentCallsMatch : IMatchingStrategy
    {
        private readonly MethodInfo expectedMethod;

        public EquivalentCallsMatch(MethodInfo expectedMethod)
        {
            this.expectedMethod = expectedMethod;
        }

        public bool Matches(MethodInfo calledMethod, IList<object> arguments)
        {
            return calledMethod.Equals(expectedMethod);
            //return new MethodInfoComparer().Equals(calledMethod, expectedMethod);
        }
    }
}