using System;
using System.Diagnostics;
using SharpMock.Core.Utility;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    [DebuggerDisplay("{Name}")]
    public class ReplaceableAssemblyInfo
    {
        public string AssemblyPath { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return Generate.Equality(this, obj, a => a.Name);
        }

        public override int GetHashCode()
        {
            return Generate.HashCode(17, 11, Name);
        }
    }
}