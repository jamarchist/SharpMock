using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
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