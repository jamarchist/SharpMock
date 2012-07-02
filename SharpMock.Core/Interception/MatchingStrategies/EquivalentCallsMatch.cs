using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class EquivalentCallsMatch : IMatchingStrategy
    {
        private readonly MethodBase expectedMethod;

        public EquivalentCallsMatch(MethodBase expectedMethod)
        {
            this.expectedMethod = expectedMethod;
        }

        public bool Matches(MethodBase calledMethod, IList<object> arguments)
        {
            return calledMethod.Equals(expectedMethod);
        }
    }
}