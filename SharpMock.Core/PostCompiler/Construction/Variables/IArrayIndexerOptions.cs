using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public interface IArrayIndexerOptions<TElementType>
    {
        IStatement Assign(IExpression expression);
        IStatement Assign(string localVariable);
    }
}