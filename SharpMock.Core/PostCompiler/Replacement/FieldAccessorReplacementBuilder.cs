using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAccessorReplacementBuilder : IReplacementBuilder
    {
        private readonly IFieldReference field;
        private readonly ReplacementRegistry registry;

        public FieldAccessorReplacementBuilder(IFieldReference field, ReplacementRegistry registry)
        {
            this.field = field;
            this.registry = registry;
        }

        public object BuildReplacement()
        {
            var replaceableField = field.AsReplaceable(ReplaceableReferenceTypes.FieldAccessor);
            if (registry.IsRegistered(replaceableField))
            //if (FieldReferenceReplacementRegistry.HasReplacementFor(field.AsReplaceable()))
            {
                //var replacementCall = FieldReferenceReplacementRegistry.GetReplacementFor(field);
                var replacementCall = registry.GetReplacement(replaceableField);
                var methodCall = new MethodCall();
                methodCall.Type = field.Type;
                methodCall.Arguments = new List<IExpression>();
                methodCall.MethodToCall = replacementCall;
                methodCall.IsStaticCall = true;

                return methodCall;
            }

            return null;
        }
    }
}