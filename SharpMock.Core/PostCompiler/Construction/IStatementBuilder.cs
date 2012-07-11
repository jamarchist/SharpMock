using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction
{
    public interface IStatementBuilder
    {
        ExpressionStatement Execute(IExpression call);
        ReturnStatement Return(IExpression expression);
        ReturnStatement Return();
    }
}
