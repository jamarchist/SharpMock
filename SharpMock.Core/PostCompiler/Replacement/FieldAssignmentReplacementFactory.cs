using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAssignmentReplacementFactory : IReplacementFactory
    {
        private readonly FieldReference field;
        private readonly ExpressionStatement assignment;
        private readonly IMetadataHost host;
        private readonly ReplacementRegistry registry;

        public FieldAssignmentReplacementFactory(FieldReference field, ExpressionStatement assignment, IMetadataHost host, ReplacementRegistry registry)
        {
            this.field = field;
            this.assignment = assignment;
            this.host = host;
            this.registry = registry;
        }

        public IReplacementRegistrar GetRegistrar()
        {
            return new FieldAssignmentReplacementRegistrar(field, registry);
        }

        public IReplacementBuilder GetBuilder()
        {
            return new FieldAssignmentReplacementBuilder(field, host, assignment, registry);
        }

        public IReplacer GetReplacer()
        {
            return new FieldAssignmentReplacer(assignment);
        }
    }
}