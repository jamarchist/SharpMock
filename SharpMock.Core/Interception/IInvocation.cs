using System;
using System.Collections.Generic;

namespace SharpMock.Core.Interception
{
    public interface IInvocation
    {
        IList<object> Arguments { get; set; }
        object Return { get; set; }
        object Target { get; set; }
        Delegate OriginalCall { get; set; }
    }
}