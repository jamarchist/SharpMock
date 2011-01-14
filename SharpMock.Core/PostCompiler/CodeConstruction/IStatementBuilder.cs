using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IStatementBuilder
    {
        ExpressionStatement Execute(MethodCall call);
    }

    public class StatementBuilder : IStatementBuilder
    {
        public ExpressionStatement Execute(MethodCall call)
        {
            var callStatement = new ExpressionStatement();
            callStatement.Expression = call;

            return callStatement;
        }
    }
}
