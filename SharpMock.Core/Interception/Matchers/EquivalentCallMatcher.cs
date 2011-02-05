using System.Reflection;

namespace SharpMock.Core.Interception
{
    public class EquivalentCallMatcher : IMethodCallMatcher
    {
        private readonly MethodInfo expectedMethod;

        public EquivalentCallMatcher(MethodInfo expectedMethod)
        {
            this.expectedMethod = expectedMethod;
        }

        public bool Matches(MethodInfo calledMethod)
        {
            return calledMethod.Equals(expectedMethod);
        }
    }
}