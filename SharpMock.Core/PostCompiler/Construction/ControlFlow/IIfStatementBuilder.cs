using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.ControlFlow
{
    public interface IIfStatementBuilder
    {
        IIfStatementOptions True(IExpression condition);
        IIfStatementOptions Not(IExpression condition);
    }
}