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

        public IClassBuilder With(VoidAction<IMethodAccessibilityOptions> method)
        {
            var newMethod = new MethodConfiguration();
            config.Methods.Add(newMethod);
            method(new MethodAcessibilityOptions(newMethod));
            return this;
        }
    }
}