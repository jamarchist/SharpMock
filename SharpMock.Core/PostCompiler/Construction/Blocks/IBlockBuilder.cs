using SharpMock.Core.PostCompiler.Construction.Declarations;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Blocks
{
    public interface IBlockBuilder
    {
        IReturnStatementBuilder Return { get; }
        IDeclarationBuilder Declare { get; }
    }
}