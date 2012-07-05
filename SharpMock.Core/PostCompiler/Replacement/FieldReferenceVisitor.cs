using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class FieldReferenceVisitor : BaseCodeTraverser
    {
        private readonly IStatement parent;
        private readonly ILogger log;

        public FieldReferenceVisitor(IStatement parent, ILogger log)
        {
            this.parent = parent;
            this.log = log;
        }

        public override void Visit(IFieldReference fieldReference)
        {
            log.WriteTrace("Visiting field: {0}.", fieldReference.Name.Value);

            if (FieldReferenceReplacementRegistry.HasReplacementFor(fieldReference.AsReplaceable()))
            {
                var replacementMethodToCall = FieldReferenceReplacementRegistry.GetReplacementFor(fieldReference);

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