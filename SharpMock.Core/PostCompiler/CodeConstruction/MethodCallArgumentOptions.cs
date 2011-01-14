using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
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
            model.ReturnType = host.PlatformType.SystemVoid;
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