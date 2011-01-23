using Microsoft.Cci;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Declarations
{
    public interface IDeclarationBuilder
    {
        IStaticDeclarationOptions<TReflectionType> Variable<TReflectionType>(string variableName);
        IDynamicDeclarationOptions Variable(string variableName, ITypeReference type);
    }
}
