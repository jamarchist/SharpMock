using Microsoft.Cci;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public interface IMethodCallArgumentOptions
    {
        IMethodCallOptions ThatReturnsVoid();
        IMethodCallOptions ThatReturns<TReturnType>();
        IMethodCallOptions ThatReturns(ITypeReference type);        
    }
}