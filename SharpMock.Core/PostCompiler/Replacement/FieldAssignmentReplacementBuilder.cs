using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAssignmentReplacementBuilder : IReplacementBuilder
    {
        private readonly FieldReference field;
        private readonly ExpressionStatement assignment;
        private readonly IMetadataHost host;

        public FieldAssignmentReplacementBuilder(FieldReference field, IMetadataHost host, ExpressionStatement assignment)
        {
            this.field = field;
            this.host = host;
            this.assignment = assignment;
        }

        public object BuildReplacement()
        {
            if (FieldAssignmentReplacementRegistry.HasReplacementFor(field.AsReplaceable()))
            {
                var replacementCall = FieldAssignmentReplacementRegistry.GetReplacementFor(field);
                var methodCall = new MethodCall();
                methodCall.Type = host.PlatformType.SystemVoid;
                methodCall.IsStaticCall = true;
                methodCall.Arguments = new List<IExpression>();
                methodCall.Arguments.Add((assignment.Expression as Assignment).Source);
                methodCall.MethodToCall = replacementCall;

                return methodCall;
            }

            return null;
        }
    }
}