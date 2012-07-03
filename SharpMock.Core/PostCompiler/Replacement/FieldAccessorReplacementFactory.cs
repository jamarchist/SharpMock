using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAccessorReplacementFactory : IReplacementFactory
    {
        private readonly FieldReference field;
        private readonly ReturnStatement firstStatement;

        public FieldAccessorReplacementFactory(FieldReference field, ReturnStatement firstStatement)
        {
            this.field = field;
            this.firstStatement = firstStatement;
        }

        public IReplacementRegistrar GetRegistrar()
        {
            return new FieldAccessorReplacementRegistrar(field);
        }

        public IReplacementBuilder GetBuilder()
        {
            return new FieldAccessorReplacementBuilder(field);
        }

        public IReplacer GetReplacer()
        {
            return new FieldAccessorReplacer(firstStatement);
        }
    }
}