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
        private readonly ReplacementRegistry registry;

        public FieldAssignmentReplacementBuilder(FieldReference field, IMetadataHost host, ExpressionStatement assignment, ReplacementRegistry registry)
        {
            this.field = field;
            this.host = host;
            this.assignment = assignment;
            this.registry = registry;
        }

        public object BuildReplacement()
        {
            var replaceableField = field.AsReplaceable(ReplaceableReferenceTypes.FieldAssignment);
            if (registry.IsRegistered(replaceableField))
            //if (FieldAssignmentReplacementRegistry.HasReplacementFor())
            {
                var replacementCall = registry.GetReplacement(replaceableField);
                //var replacementCall = FieldAssignmentReplacementRegistry.GetReplacementFor(field);
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