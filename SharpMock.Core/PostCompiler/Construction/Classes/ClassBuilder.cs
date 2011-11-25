using SharpMock.Core.PostCompiler.Construction.Fields;
using SharpMock.Core.PostCompiler.Construction.Methods;

namespace SharpMock.Core.PostCompiler.Construction.Classes
{
    internal class ClassBuilder : IClassBuilder
    {
        private readonly ClassConfiguration config;

        public ClassBuilder(ClassConfiguration config)
        {
            this.config = config;
        }

        public IClassBuilder Named(string className)
        {
            config.Name = className;
            return this;
        }

        public IClassBuilder InNamespace(string namespaceName)
        {
            config.Namespace = namespaceName;
            return this;
        }

        public IClassBuilder With(VoidAction<IMethodAccessibilityOptions> method)
        {
            var newMethod = new MethodConfiguration();
            config.Methods.Add(newMethod);
            method(new MethodAcessibilityOptions(newMethod));
            return this;
        }

        public IClassBuilder WithField(VoidAction<IFieldAccessibilityOptions> field)
        {
            var newField = new FieldConfiguration();
            config.Fields.Add(newField);
            field(new FieldAccessibilityOptions(newField));
            return this;
        }
    }
}