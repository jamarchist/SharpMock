using System.Collections.Generic;
using SharpMock.Core.PostCompiler.Construction.Methods;

namespace SharpMock.Core.PostCompiler.Construction.Classes
{
    internal class ClassConfiguration
    {
        public ClassConfiguration()
        {
            Methods = new List<MethodConfiguration>();
        }

        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public string Modifier { get; set; }
        public string Name { get; set; }
        public IList<MethodConfiguration> Methods { get; private set; }
    }
}