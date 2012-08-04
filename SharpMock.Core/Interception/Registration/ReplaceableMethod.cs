using System;
using System.Collections.Generic;
using SharpMock.Core.PostCompiler.Replacement;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    public class ReplaceableMethodInfo : IReplaceableReference
    {
        public string Name { get; set; }
        public ReplaceableTypeInfo DeclaringType { get; set; }
        public List<ReplaceableParameterInfo> Parameters { get; set; }
        public ReplaceableTypeInfo ReturnType { get; set; }

        public string ReferenceType { get { return ReplaceableReferenceTypes.Method; } }
    }
}
