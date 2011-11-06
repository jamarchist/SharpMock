using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Declarations
{
    public interface IStaticDeclarationOptions<TReflectionType>
    {
        LocalDeclarationStatement As(IExpression initialValue);
        
    }
}