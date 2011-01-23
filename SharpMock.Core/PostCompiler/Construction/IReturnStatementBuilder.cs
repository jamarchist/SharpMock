using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IReturnStatementBuilder
    {
        IReturnStatementBuilder Variable(ILocalDefinition methodCall);
        IReturnStatementBuilder OfType(ITypeReference type);
        void In(BlockStatement blockStatement);
        IReturnStatementBuilder NullOrVoid();
    }
}