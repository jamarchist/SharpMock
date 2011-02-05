using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.ControlFlow
{
    public class IfStatementBuilder : IIfStatementBuilder
    {
        public IIfStatementOptions True(IExpression condition)
        {
            var ifStatement = new ConditionalStatement();
            ifStatement.Condition = condition;

            return new IfStatementOptions(ifStatement);
        }

        public IIfStatementOptions Not(IExpression condition)
        {
            throw new NotImplementedException();
        }
    }
}
