using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class MethodCallReturnOptions : IMethodCallReturnOptions
    {
        private readonly MethodCallModel model;
        private readonly IUnitReflector reflector;
        private readonly ILocalVariableBindings locals;

        public MethodCallReturnOptions(IUnitReflector reflector, MethodCallModel model, ILocalVariableBindings locals)
        {
            this.reflector = reflector;
            this.locals = locals;
            this.model = model;
        }

        public MethodCall On(string localVariableName)
        {
            var local = locals[localVariableName];
            return MethodCall(local.Type, local);
        }

        public MethodCall On(ITypeReference type)
        {
            return MethodCall(type);
        }

        public MethodCall On<TTypeWithStaticMethod>()
        {
            return On(reflector.Get<TTypeWithStaticMethod>());
        }

        public MethodCall On(Type staticType)
        {
            return On(reflector.Get(staticType));
        }

        public MethodCall On(IExpression target)
        {
            return MethodCall(target.Type, target);
        }

        private IMethodReference GetMethodToCall(ITypeReference targetType)
        {
            if (model.IsSetter)
            {
                return reflector.From(targetType).GetPropertySetter(model.MethodName, model.ArgumentTypes[0]);
            }

            if (model.IsGetter)
            {
                return reflector.From(targetType).GetPropertyGetter(model.MethodName);
            }

            if (model.Reference == null)
            {
                return reflector.From(targetType).GetMethod(model.MethodName, model.ArgumentTypes);
            }

            return reflector.From(targetType).GetMethod(model.Reference);
        }

        private MethodCall MethodCall(ITypeReference targetType)
        {
            var methodCall = new MethodCall();
            methodCall.IsStaticCall = model.IsStatic;
            methodCall.IsVirtualCall = model.IsVirtual;
            methodCall.Arguments = model.Arguments;
            methodCall.Type = model.ReturnType;
            methodCall.MethodToCall = GetMethodToCall(targetType);

            return methodCall;
        }

        private MethodCall MethodCall(ITypeReference targetType, IExpression  thisArgument)
        {
            var methodCall = MethodCall(targetType);
            methodCall.ThisArgument = thisArgument;

            return methodCall;
        }
    }
}