using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Conversions
{
    public class Converter : IConverter
    {
        private readonly IUnitReflector reflector;

        public Converter(IUnitReflector reflector)
        {
            this.reflector = reflector;
        }

        public IConversionOptions Convert(IExpression target)
        {
            return new ConversionOptions(target, reflector);
        }

        public IExpression Box(IExpression target)
        {
            if (target.Type.IsValueType)
            {
                return new ConversionOptions(target, reflector).To<object>();
            }

            return target;
        }
    }
}