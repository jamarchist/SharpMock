using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpMock.Core.PostCompiler.Replacement;
using SharpMock.Core.Utility;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    [DebuggerDisplay("{ReturnType.FullName} {DeclaringType.FullName}.{Name}({Parameters.Count} parameters...)")]
    public class ReplaceableMethodInfo : IReplaceableReference
    {
        public string Name { get; set; }
        public ReplaceableTypeInfo DeclaringType { get; set; }
        public List<ReplaceableParameterInfo> Parameters { get; set; }
        public ReplaceableTypeInfo ReturnType { get; set; }

        public string ReferenceType { get { return ReplaceableReferenceTypes.Method; } }

        public override bool Equals(object obj)
        {
            return Generate.Equality(this, obj, 
                m => m.Name, m => m.DeclaringType, m => Generate.EquatableEnumerable(m.Parameters), m => m.ReturnType);
        }

        public override int GetHashCode()
        {
            return Generate.HashCode(7, 13, Name, DeclaringType, Generate.HashCode(29, 17, Parameters.ToArray()), ReturnType);
        }
    }
}
