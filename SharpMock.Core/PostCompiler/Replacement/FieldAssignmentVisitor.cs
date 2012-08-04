using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class FieldAssignmentVisitor : CodeTraverser
    {
        private readonly IStatement parent;
        private readonly ILogger log;
        private readonly ReplacementRegistry registry;

        public FieldAssignmentVisitor(IStatement parent, ILogger log, ReplacementRegistry registry)
        {
            this.parent = parent;
            this.log = log;
            this.registry = registry;
        }

        public override void TraverseChildren(IFieldReference fieldReference)
        {
            log.WriteTrace("Visiting field: {0}.", fieldReference.Name.Value);

            var replaceableField = fieldReference.AsReplaceable(ReplaceableReferenceTypes.FieldAssignment);

            //if (FieldReferenceReplacementRegistry.HasReplacementFor(fieldReference.AsReplaceable()))
            if (registry.IsRegistered(replaceableField))
            {
                //var replacementMethodToCall = FieldAssignmentReplacementRegistry.GetReplacementFor(fieldReference);
                var replacementMethodToCall = registry.GetReplacement(replaceableField);

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
                        var target = assignment.Target.Definition as FieldReference;
                        if (target != null)
                        {
                            // If the target is what we're visiting ...
                            if (target.ResolvedField.Equals(fieldReference.ResolvedField))
                            {
                                if (!fieldReference.ResolvedField.IsStatic)
                                {
                                    replacementExpression.Arguments.Add(assignment.Target.Instance);   
                                }

                                replacementExpression.Arguments.Add(assignment.Source);
                                expressionStatement.Expression = replacementExpression;
                            }
                        }
                    }
                }
            }
        }
    }
}