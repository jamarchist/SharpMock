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
            var generic = new GenericTypeInstanceReference();
            var closedGeneric = reflector.Get<TDelegateType>() as GenericTypeInstanceReference;
            generic.GenericType = closedGeneric.GenericType;

            generic.InternFactory = host.InternFactory;
            generic.TypeCode = PrimitiveTypeCode.NotPrimitive;
            generic.PlatformType = host.PlatformType;

            var delegateType = typeof (TDelegateType);
            var invoke = delegateType.GetMethod("Invoke");

            var parameters = invoke.GetParameters();
            
            foreach (var parameter in parameters)
            {
                generic.GenericArguments.Add(reflector.Get(parameter.ParameterType));
            }

            if (invoke.ReturnType != typeof(void))
            {
                generic.GenericArguments.Add(reflector.Get(invoke.ReturnType));
            }

            return new AnonymousMethodBodyBuilder(host, reflector, generic, invoke.ReturnType, parameters);
        }
    }
}