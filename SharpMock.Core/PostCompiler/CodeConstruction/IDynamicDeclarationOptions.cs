using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IDynamicDeclarationOptions
    {
        LocalDeclarationStatement As(IExpression initialValue);
    }
}