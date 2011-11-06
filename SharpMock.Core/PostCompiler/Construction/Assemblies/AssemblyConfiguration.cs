using System.Collections.Generic;
using SharpMock.Core.PostCompiler.Construction.Classes;

namespace SharpMock.Core.PostCompiler.Construction.Assemblies
{
    internal class AssemblyConfiguration
    {
        public AssemblyConfiguration()
        {
            Classes = new List<ClassConfiguration>();
            ReferencePaths = new List<string>();
        }

        public string Name { get; set; }
        public IList<ClassConfiguration> Classes { get; private set; }
        public IList<string> ReferencePaths { get; private set; } 
    }
}