using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IMethodCallReturnOptions
    {
        MethodCall On(string localVariableName);
        MethodCall On(ITypeReference type);
        MethodCall On<TStaticType>();
    }
}