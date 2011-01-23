using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public interface IPropertySetterValueOptions
    {
        IExpressionStatement To(IExpression value);
        IExpressionStatement To(string variableName);
    }
}