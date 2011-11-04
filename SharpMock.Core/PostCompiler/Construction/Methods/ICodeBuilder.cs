using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface ICodeBuilder
    {
        void AddLine(Function<IMethodBodyBuilder, IStatement> x);
    }
}