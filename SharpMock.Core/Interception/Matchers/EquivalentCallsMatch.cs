using System.Reflection;

namespace SharpMock.Core.Interception.Matchers
{
    public class EquivalentCallsMatch : IMatchingStrategy
    {
        private readonly MethodInfo expectedMethod;

        public EquivalentCallsMatch(MethodInfo expectedMethod)
        {
            this.expectedMethod = expectedMethod;
        }

        public bool Matches(MethodInfo calledMethod)
        {
            return calledMethod.Equals(expectedMethod);
        }
    }
}