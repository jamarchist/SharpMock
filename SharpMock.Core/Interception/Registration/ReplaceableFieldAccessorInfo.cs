using System;

namespace SharpMock.Core.Interception.Registration
{
    [Serializable]
    public class ReplaceableFieldAccessorInfo
    {
        public string Name { get; set; }
        public ReplaceableTypeInfo FieldType { get; set; }
        public ReplaceableTypeInfo DeclaringType { get; set; }
    }
}