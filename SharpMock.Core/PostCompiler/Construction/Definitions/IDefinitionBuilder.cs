using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Definitions
{
    public interface IDefinitionBuilder
    {
        LocalDefinition VariableOf<TVariableType>(string variableName);
        LocalDefinition VariableOf(string variableName, ITypeReference type);
    }
}
