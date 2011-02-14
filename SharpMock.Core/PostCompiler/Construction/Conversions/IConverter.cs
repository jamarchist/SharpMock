using Microsoft.Cci;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Conversions
{
    public interface IConverter
    {
        IConversionOptions Convert(IExpression target);
        IExpression Box(IExpression target);
    }
}
