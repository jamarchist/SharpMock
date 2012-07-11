using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction
{
    public class StatementBuilder : IStatementBuilder
    {
        public ExpressionStatement Execute(IExpression call)
        {
            var callStatement = new ExpressionStatement();
            callStatement.Expression = call;

            return callStatement;
        }

        public ReturnStatement Return(IExpression expression)
        {
            var @return = new ReturnStatement();
            @return.Expression = expression;

            return @return;
        }

        public ReturnStatement Return()
        {
            return new ReturnStatement();
        }
    }
}