using System;
using System.Diagnostics;
using SharpMock.Core.Utility;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    [DebuggerDisplay("{DeclaringType.FullName}.{Name}")]
    public class ReplaceableFieldInfo : IReplaceableReference
    {
        public ReplaceableFieldInfo() { }

        public ReplaceableFieldInfo(string referenceType)
        {
            ReferenceType = referenceType;
        }

        public string Name { get; set; }
        public ReplaceableTypeInfo FieldType { get; set; }
        public ReplaceableTypeInfo DeclaringType { get; set; }
        public string ReferenceType { get; set; }

        public override bool Equals(object obj)
        {
            return Generate.Equality(this, obj, 
                f => f.Name, f => f.FieldType, f => f.DeclaringType, f => f.ReferenceType);
        }

        public override int GetHashCode()
        {
            return Generate.HashCode(7, 13, Name, FieldType, DeclaringType, ReferenceType);
        }
    }
}