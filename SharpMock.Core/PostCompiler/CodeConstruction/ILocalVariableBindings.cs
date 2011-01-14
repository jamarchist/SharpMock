using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface ILocalVariableBindings
    {
        void AddBinding(string localVariableName, ILocalDefinition definition, ITypeReference type);
        IBoundExpression this[string localVariableName] { get; }
    }
}
