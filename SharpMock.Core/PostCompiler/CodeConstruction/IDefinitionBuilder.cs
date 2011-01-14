using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IDefinitionBuilder
    {
        LocalDefinition VariableOf<TVariableType>(string variableName);
        LocalDefinition VariableOf(string variableName, ITypeReference type);
    }
}
