using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction
{
    public class TypeOperatorBuilder : ITypeOperatorBuilder
    {
        private readonly IUnitReflector reflector;

        public TypeOperatorBuilder(IUnitReflector reflector)
        {
            this.reflector = reflector;
        }

        public IExpression TypeOf(ITypeReference typeReference)
        {
            var typeOf = new TypeOf();
            typeOf.TypeToGet = typeReference;
            typeOf.Type = reflector.Get<Type>();

            return typeOf;
        }
    }
}