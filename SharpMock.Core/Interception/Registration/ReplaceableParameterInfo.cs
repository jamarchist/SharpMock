using System;
using System.Diagnostics;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    [DebuggerDisplay("{Index}: {ParameterType.FullName} {Name}")]
    public class ReplaceableParameterInfo
    {
        public ReplaceableTypeInfo ParameterType { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
    }
}