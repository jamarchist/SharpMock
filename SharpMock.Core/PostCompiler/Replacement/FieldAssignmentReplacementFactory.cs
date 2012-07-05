using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAssignmentReplacementFactory : IReplacementFactory
    {
        private readonly FieldReference field;
        private readonly ExpressionStatement assignment;

        public FieldAssignmentReplacementFactory(FieldReference field, ExpressionStatement assignment)
        {
            this.field = field;
            this.assignment = assignment;
        }

        public IReplacementRegistrar GetRegistrar()
        {
            throw new System.NotImplementedException();
        }

        public IReplacementBuilder GetBuilder()
        {
            throw new System.NotImplementedException();
        }

        public IReplacer GetReplacer()
        {
            throw new System.NotImplementedException();
        }
    }
}