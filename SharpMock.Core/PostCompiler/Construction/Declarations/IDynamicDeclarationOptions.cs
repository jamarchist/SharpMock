using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Declarations
{
    public interface IDynamicDeclarationOptions
    {
        LocalDeclarationStatement As(IExpression initialValue);
    }
}