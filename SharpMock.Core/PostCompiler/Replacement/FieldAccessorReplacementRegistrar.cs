using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAccessorReplacementRegistrar : IReplacementRegistrar
    {
        private readonly FieldReference field;

        public FieldAccessorReplacementRegistrar(FieldReference field)
        {
            this.field = field;
        }

        public void RegisterReplacement()
        {
            if (field != null)
            {
                FieldReferenceReplacementRegistry.AddFieldToIntercept(field);
            }
        }
    }
}