using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAccessorReplacementBuilder : IReplacementBuilder
    {
        private readonly IFieldReference field;

        public FieldAccessorReplacementBuilder(IFieldReference field)
        {
            this.field = field;
        }

        public object BuildReplacement()
        {
            if (FieldReferenceReplacementRegistry.HasReplacementFor(field.AsReplaceable()))
            {
                var replacementCall = FieldReferenceReplacementRegistry.GetReplacementFor(field);
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