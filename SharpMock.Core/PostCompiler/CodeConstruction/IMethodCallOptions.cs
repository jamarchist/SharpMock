using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IMethodCallOptions
    {
        IMethodCallReturnOptions WithArguments(params IExpression[] arguments);
        IMethodCallReturnOptions WithArguments(params string[] arguments);
        IMethodCallReturnOptions WithNoArguments();
    }
}