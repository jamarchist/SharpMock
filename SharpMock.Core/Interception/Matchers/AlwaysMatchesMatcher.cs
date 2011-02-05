using System.Reflection;

namespace SharpMock.Core.Interception.Matchers
{
    public class AlwaysMatchesMatcher : IMethodCallMatcher
    {
        public bool Matches(MethodInfo calledMethod)
        {
            return true;
        }
    }
}
