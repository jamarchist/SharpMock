using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAssignmentReplacementRegistrar : IReplacementRegistrar
    {
        private readonly FieldReference field;

        public FieldAssignmentReplacementRegistrar(FieldReference field)
        {
            this.field = field;
        }

        public void RegisterReplacement()
        {
            if (field != null)
            {
                FieldAssignmentReplacementRegistry.AddFieldToIntercept(field);
            }
        }
    }
}