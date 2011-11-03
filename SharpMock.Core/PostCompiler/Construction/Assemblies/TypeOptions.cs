using SharpMock.Core.PostCompiler.Construction.Classes;

namespace SharpMock.Core.PostCompiler.Construction.Assemblies
{
    internal class TypeOptions : ITypeOptions
    {
        private readonly ClassConfiguration config;

        public TypeOptions(ClassConfiguration config)
        {
            this.config = config;
        }

        public IClassAccessiblityOptions Class
        {
            get { return new ClassAccessibilityOptions(config); }
        }
    }
}