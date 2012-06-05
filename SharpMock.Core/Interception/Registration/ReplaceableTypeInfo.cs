using System;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    public class ReplaceableTypeInfo
    {
        public ReplaceableAssemblyInfo Assembly { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
    }
}