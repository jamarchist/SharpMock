using System.Collections.Generic;
using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class MethodCallOptions : IMethodCallOptions
    {
        private readonly IMetadataHost host;
        private readonly IUnitReflector reflector;
        private readonly MethodCallModel model;
        private readonly ILocalVariableBindings locals;

        public MethodCallOptions(IMetadataHost host, IUnitReflector reflector, MethodCallModel model, ILocalVariableBindings locals)
        {
            this.model = model;
            this.locals = locals;
            this.reflector = reflector;
            this.host = host;
        }

        public IMethodCallReturnOptions WithArguments(params IExpression[] arguments)
        {
            model.Arguments = new List<IExpression>(arguments);
            return new MethodCallReturnOptions(reflector, model, locals);
        }

        public IMethodCallReturnOptions WithArguments(params string[] arguments)
        {
            var boundArguments = new List<IExpression>();
            foreach (var argument in arguments)
            {
                boundArguments.Add(locals[argument]);
            }

            model.Arguments = boundArguments;

            return new MethodCallReturnOptions(reflector, model, locals);
        }

        public IMethodCallReturnOptions WithNoArguments()
        {
            model.Arguments = new List<IExpression>();
            return new MethodCallReturnOptions(reflector, model, locals);
        }
    }
}