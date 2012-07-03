using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class ConstructorReplacementFactory : IReplacementFactory
    {
        private readonly ReturnStatement firstStatement;
        private readonly CreateObjectInstance constructor;

        public ConstructorReplacementFactory(CreateObjectInstance constructor, ReturnStatement firstStatement)
        {
            this.constructor = constructor;
            this.firstStatement = firstStatement;
        }

        public IReplacementRegistrar GetRegistrar()
        {
            return new MethodCallReplacementRegistrar(constructor);
        }

        public IReplacementBuilder GetBuilder()
        {
            return new ConstructorReplacementBuilder(constructor);
        }

        public IReplacer GetReplacer()
        {
            return new ConstructorReplacer(firstStatement);
        }
    }
}