using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IConversionOptions
    {
        IExpression To(ITypeReference type);
        IExpression To<TReflectionType>();
    }

    public interface IConverter
    {
        IConversionOptions Convert(IExpression target);
        IExpression Box(IExpression target);
    }

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
