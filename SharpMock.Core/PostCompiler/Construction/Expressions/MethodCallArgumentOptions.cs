using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class MethodCallArgumentOptions : IMethodCallArgumentOptions
    {
        private readonly MethodCallModel model;
        private readonly IUnitReflector reflector;
        private readonly ILocalVariableBindings locals;
        private readonly IMetadataHost host;

        public MethodCallArgumentOptions(IUnitReflector reflector, MethodCallModel model, ILocalVariableBindings locals, IMetadataHost host)
        {
            this.reflector = reflector;
            this.host = host;
            this.locals = locals;
            this.model = model;
        }

        public IMethodCallOptions ThatReturnsVoid()
        {
            model.ReturnType = reflector.Get(typeof(void));
            return new MethodCallOptions(host, reflector, model, locals);
        }

        public IMethodCallOptions ThatReturns<TReturnType>()
        {
            model.ReturnType = reflector.Get<TReturnType>();
            return new MethodCallOptions(host, reflector, model, locals);
        }

        public IMethodCallOptions ThatReturns(ITypeReference type)
        {
            model.ReturnType = type;
            return new MethodCallOptions(host, reflector, model, locals);
        }
    }
}