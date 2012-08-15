using System;
using System.Diagnostics;
using SharpMock.Core.Utility;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    [DebuggerDisplay("{Index}: {ParameterType.FullName} {Name}")]
    public class ReplaceableParameterInfo
    {
        public ReplaceableTypeInfo ParameterType { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return Generate.Equality(this, obj, p => p.Index, p => p.ParameterType);
        }

        public override int GetHashCode()
        {
            return Generate.HashCode(31, 7, Index, ParameterType);
        }
    }
}