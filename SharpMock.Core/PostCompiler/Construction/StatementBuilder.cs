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
    }
}