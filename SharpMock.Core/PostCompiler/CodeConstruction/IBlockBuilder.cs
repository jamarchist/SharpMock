namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IBlockBuilder
    {
        IReturnStatementBuilder Return { get; }
        IDeclarationBuilder Declare { get; }
    }
}