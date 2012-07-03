using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class FieldAccessorReplacer : IReplacer
    {
        private readonly ReturnStatement firstStatement;

        public FieldAccessorReplacer(ReturnStatement firstStatement)
        {
            this.firstStatement = firstStatement;
        }

        public void ReplaceWith(object replacement)
        {
            if (replacement != null)
            {
                firstStatement.Expression = replacement as IExpression;
            }
        }
    }
}