using System;
using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class MethodCallBuilder : IMethodCallBuilder
    {
        private readonly IMetadataHost host;
        private readonly IUnitReflector reflector;
        private readonly ILocalVariableBindings locals;

        public MethodCallBuilder(IMetadataHost host, IUnitReflector reflector, ILocalVariableBindings locals)
        {
            this.host = host;
            this.locals = locals;
            this.reflector = reflector;
        }


        public IMethodCallArgumentOptions Method(string methodName, params Type[] argumentTypes)
        {
            return Options(methodName, argumentTypes, null, false, false, false, false);
        }

        public IMethodCallArgumentOptions VirtualMethod(string methodName, params Type[] argumentTypes)
        {
            return Options(methodName, argumentTypes, null, false, true, false, false);
        }

        public IMethodCallArgumentOptions StaticMethod(string methodName, params Type[] argumentTypes)
        {
            return Options(methodName, argumentTypes, null, true, false, false, false);
        }

        public IMethodCallArgumentOptions Method(IMethodReference method)
        {
            return Options(null, null, method, false, false, false, false);
        }

        public IMethodCallArgumentOptions VirtualMethod(IMethodReference method)
        {
            return Options(null, null, method, false, true, false, false);
        }

        public IMethodCallArgumentOptions StaticMethod(IMethodReference method)
        {
            return Options(null, null, method, true, false, false, false);
        }

        private IMethodCallOptions PropertySetter<TPropertyType>(string propertyName, bool isStatic, bool isVirtual)
        {
            var propertyType = typeof (TPropertyType);
            var options = Options(propertyName, new Type[] {propertyType}, null, isStatic, isVirtual, false, true);
            return options.ThatReturnsVoid();           
        }

        private IMethodCallReturnOptions PropertyGetter<TPropertyType>(string propertyName, bool isStatic, bool isVirtual)
        {
            var options = Options(propertyName, new Type[] {}, null, isStatic, isVirtual, true, false);
            return options.ThatReturns<TPropertyType>().WithNoArguments();            
        }

        public IMethodCallOptions PropertySetter<TPropertyType>(string propertyName)
        {
            return PropertySetter<TPropertyType>(propertyName, false, false);
        }

        public IMethodCallReturnOptions PropertyGetter<TPropertyType>(string propertyName)
        {
            return PropertyGetter<TPropertyType>(propertyName, false, false);
        }

        public IMethodCallOptions StaticPropertySetter<TPropertyType>(string propertyName)
        {
            return PropertySetter<TPropertyType>(propertyName, true, false);
        }

        public IMethodCallReturnOptions StaticPropertyGetter<TPropertyType>(string propertyName)
        {
            return PropertyGetter<TPropertyType>(propertyName, true, false);
        }

        public IMethodCallOptions VirtualPropertySetter<TPropertyType>(string propertyName)
        {
            return PropertySetter<TPropertyType>(propertyName, false, true);
        }

        public IMethodCallReturnOptions VirtualPropertyGetter<TPropertyType>(string propertyName)
        {
            return PropertyGetter<TPropertyType>(propertyName, false, true);
        }

        private IMethodCallArgumentOptions Options(string name, Type[] argumentTypes, IMethodReference reference, bool isStatic, bool isVirtual, bool isGetter, bool isSetter)
        {
            var model = new MethodCallModel();
            model.MethodName = name;
            model.ArgumentTypes = argumentTypes;
            model.Reference = reference;
            model.IsStatic = isStatic;
            model.IsVirtual = isVirtual;
            model.IsGetter = isGetter;
            model.IsSetter = isSetter;

            return new MethodCallArgumentOptions(reflector, model, locals, host);
        }
    }
}