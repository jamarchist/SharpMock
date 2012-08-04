using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class ConstructorReplacementFactory : IReplacementFactory
    {
        private readonly ReturnStatement firstStatement;
        private readonly CreateObjectInstance constructor;
        private readonly ReplacementRegistry registry;

        public ConstructorReplacementFactory(CreateObjectInstance constructor, ReturnStatement firstStatement, ReplacementRegistry registry)
        {
            this.constructor = constructor;
            this.firstStatement = firstStatement;
            this.registry = registry;
        }

        public IReplacementRegistrar GetRegistrar()
        {
            return new MethodCallReplacementRegistrar(constructor, registry);
        }

        public IReplacementBuilder GetBuilder()
        {
            return new ConstructorReplacementBuilder(constructor, registry);
        }

        public IReplacer GetReplacer()
        {
            return new ConstructorReplacer(firstStatement);
        }
    }
}