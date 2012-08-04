using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAccessorReplacementRegistrar : IReplacementRegistrar
    {
        private readonly FieldReference field;
        private readonly ReplacementRegistry registry;

        public FieldAccessorReplacementRegistrar(FieldReference field, ReplacementRegistry registry)
        {
            this.field = field;
            this.registry = registry;
        }

        public void RegisterReplacement()
        {
            if (field != null)
            {
                registry.RegisterReference(field.AsReplaceable(ReplaceableReferenceTypes.FieldAccessor));
                //FieldReferenceReplacementRegistry.AddFieldToIntercept(field);
            }
        }
    }
}