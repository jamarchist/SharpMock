using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Conversions
{
    public interface IConversionOptions
    {
        IExpression To(ITypeReference type);
        IExpression To<TReflectionType>();
    }
}