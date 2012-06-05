using System;
using System.Collections.Generic;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    public class ReplaceableMethodInfo
    {
        public string Name { get; set; }
        public ReplaceableTypeInfo DeclaringType { get; set; }
        public List<ReplaceableParameterInfo> Parameters { get; set; }
        public ReplaceableTypeInfo ReturnType { get; set; }
    }
}
