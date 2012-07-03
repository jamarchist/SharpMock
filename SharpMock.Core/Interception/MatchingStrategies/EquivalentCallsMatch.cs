using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class EquivalentCallsMatch : IMatchingStrategy
    {
        private readonly MemberInfo expectedMethod;

        public EquivalentCallsMatch(MemberInfo expectedMethod)
        {
            this.expectedMethod = expectedMethod;
        }

        public bool Matches(MemberInfo calledMethod, IList<object> arguments)
        {
            return calledMethod.Equals(expectedMethod);
        }
    }
}