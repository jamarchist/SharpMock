using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class AlwaysMatches : IMatchingStrategy
    {
        public bool Matches(MethodBase calledMethod, IList<object> arguments)
        {
            return true;
        }
    }
}
