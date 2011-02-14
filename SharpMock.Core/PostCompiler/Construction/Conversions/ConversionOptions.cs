using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Conversions
{
    public class ConversionOptions : IConversionOptions
    {
        private readonly IExpression target;
        private readonly IUnitReflector reflector;

        public ConversionOptions(IExpression target, IUnitReflector reflector)
        {
            this.target = target;
            this.reflector = reflector;
        }

        public IExpression To(ITypeReference type)
        {
            var conversion = new Conversion();
            conversion.ValueToConvert = target;
            conversion.TypeAfterConversion = type;

            return conversion;
        }

        public IExpression To<TReflectionType>()
        {
            var conversion = new Conversion();
            conversion.ValueToConvert = target;
            conversion.TypeAfterConversion = reflector.Get<TReflectionType>();

            return conversion;
        }
    }
}