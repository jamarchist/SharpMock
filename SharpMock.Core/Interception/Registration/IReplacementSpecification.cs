using System.Collections.Generic;

namespace SharpMock.Core.Interception.Registration
{
    public interface IReplacementSpecification
    {
        IList<ReplaceableMethodInfo> GetMethodsToReplace();
    }
}
