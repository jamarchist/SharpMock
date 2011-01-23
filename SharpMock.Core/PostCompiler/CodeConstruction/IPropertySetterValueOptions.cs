using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core
{
    public interface IPropertySetterValueOptions
    {
        IExpressionStatement To(IExpression value);
        IExpressionStatement To(string variableName);
    }
}