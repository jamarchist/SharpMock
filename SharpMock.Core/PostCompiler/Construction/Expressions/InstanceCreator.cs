using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class InstanceCreator : IInstanceCreator
    {
        private readonly IUnitReflector reflector;

        public InstanceCreator(IUnitReflector reflector)
        {
            this.reflector = reflector;
        }

        public CreateObjectInstance New(ITypeReference type, params ITypeReference[] constructorParameters)
        {
            var createObjectInstance = new CreateObjectInstance();
            createObjectInstance.Type = type.ResolvedType;
            createObjectInstance.MethodToCall = reflector.From(type).GetConstructor(constructorParameters);

            return createObjectInstance;
        }

        public CreateObjectInstance New<TReflectionType>()
        {
            var createObjectInstance = new CreateObjectInstance();
            var objectType = reflector.Get<TReflectionType>();
            createObjectInstance.Type = objectType.ResolvedType;
            createObjectInstance.MethodToCall = reflector.From<TReflectionType>().GetConstructor(Type.EmptyTypes);

            return createObjectInstance;
        }

        public DefaultValue Default<TReflectionType>()
        {
            var defaultValue = new DefaultValue();
            defaultValue.DefaultValueType = reflector.Get<TReflectionType>();
            defaultValue.Type = defaultValue.DefaultValueType;

            return defaultValue;
        }

        public CreateArray NewArray<TReflectionType>(int size)
        {
            var createArray = new CreateArray();
            var objectType = reflector.Get<TReflectionType[]>();
            var elementType = reflector.Get<TReflectionType>();
            createArray.Type = objectType.ResolvedType;
            createArray.ElementType = elementType;

            var sizeConstant = new CompileTimeConstant();
            sizeConstant.Type = reflector.Get<int>();
            sizeConstant.Value = size;

            createArray.Sizes.Add(sizeConstant);

            return createArray;
        }
    }
}