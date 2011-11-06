using System;
using System.Collections.Generic;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal class MethodConfiguration
    {
        public MethodConfiguration()
        {
            Parameters = new List<KeyValuePair<string, Type>>();
        }

        public string Modifier { get; set; }
        public bool IsStatic { get; set; }
        public string Name { get; set; }
        public Type ReturnType { get; set; }
        public VoidAction<ICodeBuilder> MethodBody { get; set; }
        public IList<KeyValuePair<string, Type>> Parameters { get; private set; }
    }
}