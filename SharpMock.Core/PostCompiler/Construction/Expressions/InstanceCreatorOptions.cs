using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class InstanceCreatorOptions : IInstanceCreatorOptions
    {
        private readonly IUnitReflector reflector;
        private readonly ILocalVariableBindings locals;
        private readonly ITypeReference type;
        private readonly ITypeReference[] constructorParameters;

        public InstanceCreatorOptions(IUnitReflector reflector, ILocalVariableBindings locals, ITypeReference type, params ITypeReference[] constructorParameters)
        {
            this.type = type;
            this.locals = locals;
            this.reflector = reflector;
            this.constructorParameters = constructorParameters;
        }

        public CreateObjectInstance WithArguments(params IExpression[] arguments)
        {
            var createObjectInstance = new CreateObjectInstance();
            createObjectInstance.Type = type.ResolvedType;
            createObjectInstance.MethodToCall = reflector.From(type).GetConstructor(constructorParameters);
            createObjectInstance.Arguments = new List<IExpression>(arguments);

            return createObjectInstance;
        }

        public CreateObjectInstance WithArguments(params string[] arguments)
        {
            var argumentList = new List<IExpression>();
            foreach (var local in arguments)
            {
                argumentList.Add(locals[local]);
            }

            return WithArguments(argumentList.ToArray());
        }

        public CreateObjectInstance WithNoArguments()
        {
            return WithArguments(new List<IExpression>().ToArray());
        }
    }
}