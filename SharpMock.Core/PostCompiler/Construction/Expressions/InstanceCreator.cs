using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class InstanceCreator : IInstanceCreator
    {
        private readonly IUnitReflector reflector;
        private readonly ILocalVariableBindings locals;

        public InstanceCreator(IUnitReflector reflector, ILocalVariableBindings locals)
        {
            this.reflector = reflector;
            this.locals = locals;
        }

        public IInstanceCreatorOptions New(ITypeReference type, params ITypeReference[] constructorParameters)
        {
            return new InstanceCreatorOptions(reflector, locals, type, constructorParameters);
        }

        public CreateObjectInstance New<TReflectionType>()
        {
            var objectType = reflector.Get<TReflectionType>();
            return new InstanceCreatorOptions(reflector, locals, objectType).WithNoArguments();
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