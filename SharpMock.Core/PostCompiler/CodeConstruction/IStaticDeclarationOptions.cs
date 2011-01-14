using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IStaticDeclarationOptions<TReflectionType>
    {
        LocalDeclarationStatement As(IExpression initialValue);
    }
}