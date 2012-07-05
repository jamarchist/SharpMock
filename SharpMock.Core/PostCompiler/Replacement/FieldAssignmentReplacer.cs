using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAssignmentReplacer : IReplacer
    {
        private readonly ExpressionStatement assignment;

        public FieldAssignmentReplacer(ExpressionStatement assignment)
        {
            this.assignment = assignment;
        }

        public void ReplaceWith(object replacement)
        {
            if (replacement != null)
            {
                assignment.Expression = replacement as IExpression;
            }
        }
    }
}