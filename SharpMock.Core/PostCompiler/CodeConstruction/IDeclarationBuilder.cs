using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IDeclarationBuilder
    {
        IStaticDeclarationOptions<TReflectionType> Variable<TReflectionType>(string variableName);
        IDynamicDeclarationOptions Variable(string variableName, ITypeReference type);
    }
}
