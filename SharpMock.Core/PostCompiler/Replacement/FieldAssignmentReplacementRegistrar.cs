using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAssignmentReplacementRegistrar : IReplacementRegistrar
    {
        private readonly FieldReference field;
        private readonly ReplacementRegistry registry;

        public FieldAssignmentReplacementRegistrar(FieldReference field, ReplacementRegistry registry)
        {
            this.field = field;
            this.registry = registry;
        }

        public void RegisterReplacement()
        {
            if (field != null)
            {
                registry.RegisterReference(field.AsReplaceable(ReplaceableReferenceTypes.FieldAssignment));
                //FieldAssignmentReplacementRegistry.AddFieldToIntercept(field);
            }
        }
    }
}