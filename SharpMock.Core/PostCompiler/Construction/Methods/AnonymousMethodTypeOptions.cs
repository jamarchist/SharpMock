using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal class AnonymousMethodTypeOptions : IAnonymousMethodTypeOptions
    {
        private readonly IUnitReflector reflector;
        private readonly IMetadataHost host;
        private readonly SharpMockTypes sharpMock;

        public AnonymousMethodTypeOptions(IMetadataHost host, IUnitReflector reflector)
        {
            this.host = host;
            this.reflector = reflector;
            sharpMock = new SharpMockTypes(host);
        }

        public IAnonymousMethodBodyBuilder Func(params ITypeReference[] typeParameters)
        {
            var openGeneric = sharpMock.Functions[typeParameters.Length - 1];
            var closedGenericType = new GenericTypeInstanceReference();
            closedGenericType.GenericType = openGeneric;

            var namedTypeParameters = new List<KeyValuePair<string, ITypeReference>>();
            for (var pIndex = 0; pIndex < typeParameters.Length; pIndex++)
            {
                closedGenericType.GenericArguments.Add(typeParameters[pIndex]);
                namedTypeParameters.Add(new KeyValuePair<string, ITypeReference>(string.Format("p{0}", pIndex), typeParameters[pIndex]));
            }

            return new AnonymousMethodBodyBuilder(host, reflector, closedGenericType, 
                typeParameters[typeParameters.Length - 1], namedTypeParameters.GetRange(0, namedTypeParameters.Count - 1).ToArray());
        }

        public IAnonymousMethodBodyBuilder Action(params ITypeReference[] typeParameters)
        {
            if (typeParameters.Length == 0)
            {
                return new AnonymousMethodBodyBuilder(host, reflector, sharpMock.Actions[0], 
                    host.PlatformType.SystemVoid, new KeyValuePair<string, ITypeReference>[]{});
            }

            var openGeneric = sharpMock.Actions[typeParameters.Length];
            var closedGenericType = new GenericTypeInstanceReference();
            closedGenericType.GenericType = openGeneric;

            var namedTypeParameters = new List<KeyValuePair<string, ITypeReference>>();
            for (var pIndex = 0; pIndex < typeParameters.Length; pIndex++)
            {
                closedGenericType.GenericArguments.Add(typeParameters[pIndex]);
                namedTypeParameters.Add(new KeyValuePair<string, ITypeReference>(string.Format("p{0}", pIndex), typeParameters[pIndex]));
            }

            return new AnonymousMethodBodyBuilder(host, reflector, closedGenericType,
                host.PlatformType.SystemVoid, namedTypeParameters.GetRange(0, namedTypeParameters.Count).ToArray());
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