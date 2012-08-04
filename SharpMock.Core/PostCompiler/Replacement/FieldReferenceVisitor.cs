using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class FieldReferenceVisitor : CodeTraverser
    {
        private readonly IStatement parent;
        private readonly ILogger log;
        private readonly ReplacementRegistry registry;

        public FieldReferenceVisitor(IStatement parent, ILogger log, ReplacementRegistry registry)
        {
            this.parent = parent;
            this.log = log;
            this.registry = registry;
        }

        public override void TraverseChildren(IFieldReference fieldReference)
        {
            log.WriteTrace("Visiting field: {0}.", fieldReference.Name.Value);

            var replaceableField = fieldReference.AsReplaceable(ReplaceableReferenceTypes.FieldAccessor);

            if (registry.IsRegistered(replaceableField))
            //if (FieldReferenceReplacementRegistry.HasReplacementFor(fieldReference.AsReplaceable()))
            {
                var replacementMethodToCall = registry.GetReplacement(replaceableField);
                //var replacementMethodToCall = FieldReferenceReplacementRegistry.GetReplacementFor(fieldReference);

                var replacementExpression = new MethodCall();
                replacementExpression.Type = replacementMethodToCall.Type;
                replacementExpression.Arguments = new List<IExpression>();
                replacementExpression.MethodToCall = replacementMethodToCall;
                replacementExpression.IsStaticCall = true;

                var expressionStatement = parent as ExpressionStatement;
                if (expressionStatement != null)
                {
                    var assignment = expressionStatement.Expression as Assignment;
                    if (assignment != null)
                    {
                        var source = assignment.Source as BoundExpression;
                        if (source != null)
                        {
                            var assignmentSource = source.Definition as FieldReference;
                            if (assignmentSource != null)
                            {
                                if (fieldReference.ResolvedField.Equals(assignmentSource.ResolvedField))
                                {
                                    assignment.Source = replacementExpression;
                                }                                
                            }
                        }
                    }
                }

                var returnStatement = parent as ReturnStatement;
                if (returnStatement != null)
                {
                    returnStatement.Expression = replacementExpression;
                }
            }
        }
    }
}