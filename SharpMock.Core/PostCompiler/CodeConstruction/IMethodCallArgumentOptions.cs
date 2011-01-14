using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IMethodCallArgumentOptions
    {
        IMethodCallOptions ThatReturnsVoid();
        IMethodCallOptions ThatReturns<TReturnType>();
        IMethodCallOptions ThatReturns(ITypeReference type);        
    }
}