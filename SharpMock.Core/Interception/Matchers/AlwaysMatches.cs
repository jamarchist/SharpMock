using System.Reflection;

namespace SharpMock.Core.Interception.Matchers
{
    public class AlwaysMatches : IMatchingStrategy
    {
        public bool Matches(MethodInfo calledMethod)
        {
            return true;
        }
    }
}
