using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core
{
    public interface IMethodCallArgumentOptions
    {
        IExpressionStatement WithArguments(params string[] arguments);
        IExpressionStatement WithArguments(params IExpression[] arguments);
    }
}