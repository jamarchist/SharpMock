using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public interface IInstanceCreatorOptions
    {
        CreateObjectInstance WithArguments(params IExpression[] arguments);
        CreateObjectInstance WithArguments(params string[] arguments);
        CreateObjectInstance WithNoArguments();
    }
}