using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public interface IParameterBindings
    {
        void AddBinding(IParameterDefinition definition);
        IBoundExpression this[string parameterName] { get; }
    }
}