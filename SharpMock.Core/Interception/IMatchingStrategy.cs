using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception
{
    public interface IMatchingStrategy
    {
        bool Matches(MethodInfo calledMethod, IList<object> arguments);
    }
}
