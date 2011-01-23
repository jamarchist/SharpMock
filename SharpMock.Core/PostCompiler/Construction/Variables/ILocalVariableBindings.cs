using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public interface ILocalVariableBindings
    {
        void AddBinding(string localVariableName, ILocalDefinition definition, ITypeReference type);
        IBoundExpression this[string localVariableName] { get; }
    }
}
