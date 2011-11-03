using System;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal class MethodConfiguration
    {
        public string Modifier { get; set; }
        public bool IsStatic { get; set; }
        public string Name { get; set; }
        public Type ReturnType { get; set; }
    }
}