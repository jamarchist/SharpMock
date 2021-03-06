using System;
using System.Diagnostics;
using System.Xml.Serialization;
using SharpMock.Core.Utility;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    [DebuggerDisplay("{FullName} | {Assembly.Name}")]
    public class ReplaceableTypeInfo
    {
        public ReplaceableAssemblyInfo Assembly { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }

        [XmlIgnore]
        public string FullName { get { return String.Format("{0}.{1}", Namespace, Name); } }

        public override bool Equals(object obj)
        {
            return Generate.Equality(this, obj,
                t => t.Namespace, t => t.Name, t => t.Assembly);
        }

        public override int GetHashCode()
        {
            return Generate.HashCode(23, 11, Namespace, Name, Assembly);
        }
    }
}