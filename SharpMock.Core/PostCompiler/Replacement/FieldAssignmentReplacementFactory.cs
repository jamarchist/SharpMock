using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAssignmentReplacementFactory : IReplacementFactory
    {
        private readonly FieldReference field;
        private readonly ExpressionStatement assignment;
        private readonly IMetadataHost host;

        public FieldAssignmentReplacementFactory(FieldReference field, ExpressionStatement assignment, IMetadataHost host)
        {
            this.field = field;
            this.assignment = assignment;
            this.host = host;
        }

        public IReplacementRegistrar GetRegistrar()
        {
            return new FieldAssignmentReplacementRegistrar(field);
        }

        public IReplacementBuilder GetBuilder()
        {
            return new FieldAssignmentReplacementBuilder(field, host, assignment);
        }

        public IReplacer GetReplacer()
        {
            return new FieldAssignmentReplacer(assignment);
        }
    }
}