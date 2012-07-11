using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal class AnonymousMethodTypeOptions : IAnonymousMethodTypeOptions
    {
        private readonly IUnitReflector reflector;
        private readonly IMetadataHost host;

        public AnonymousMethodTypeOptions(IMetadataHost host, IUnitReflector reflector)
        {
            this.host = host;
            this.reflector = reflector;
        }

        public IAnonymousMethodBodyBuilder Of<TDelegateType>()
        {
            var delegateType = typeof(TDelegateType);
            var invoke = delegateType.GetMethod("Invoke");

            var parameters = invoke.GetParameters();
            
            return new AnonymousMethodBodyBuilder(host, reflector, reflector.Get<TDelegateType>(), invoke.ReturnType, parameters);
        }

        private ITypeReference GetTypeReference<TDelegateType>()
        {
            var typeReference = reflector.Get<TDelegateType>();
            if (typeReference is GenericTypeInstanceReference)
            {
                var generic = typeReference as GenericTypeInstanceReference;

            }
            else
            {
                
            }

            return typeReference;
        }
    }
}