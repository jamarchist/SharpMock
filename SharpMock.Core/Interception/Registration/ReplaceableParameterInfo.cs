using System;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    public class ReplaceableParameterInfo
    {
        public ReplaceableTypeInfo ParameterType { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
    }
}