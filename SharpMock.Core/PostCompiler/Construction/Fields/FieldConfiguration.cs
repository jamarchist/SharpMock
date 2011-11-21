using System;

namespace SharpMock.Core.PostCompiler.Construction.Fields
{
    internal class FieldConfiguration
    {
        public Accessibility Accessibility { get; set; }
        public string Name { get; set; }
        public Type FieldType { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsStatic { get; set; }
    }
}
