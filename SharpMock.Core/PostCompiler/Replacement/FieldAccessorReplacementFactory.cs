using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAccessorReplacementFactory : IReplacementFactory
    {
        private readonly FieldReference field;
        private readonly ReturnStatement firstStatement;
        private readonly ReplacementRegistry registry;

        public FieldAccessorReplacementFactory(FieldReference field, ReturnStatement firstStatement, ReplacementRegistry registry)
        {
            this.field = field;
            this.firstStatement = firstStatement;
            this.registry = registry;
        }

        public IReplacementRegistrar GetRegistrar()
        {
            return new FieldAccessorReplacementRegistrar(field, registry);
        }

        public IReplacementBuilder GetBuilder()
        {
            return new FieldAccessorReplacementBuilder(field, registry);
        }

        public IReplacer GetReplacer()
        {
            return new FieldAccessorReplacer(firstStatement);
        }
    }
}