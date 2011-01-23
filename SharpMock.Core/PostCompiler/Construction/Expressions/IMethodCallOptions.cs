using Microsoft.Cci;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public interface IMethodCallOptions
    {
        IMethodCallReturnOptions WithArguments(params IExpression[] arguments);
        IMethodCallReturnOptions WithArguments(params string[] arguments);
        IMethodCallReturnOptions WithNoArguments();
    }
}