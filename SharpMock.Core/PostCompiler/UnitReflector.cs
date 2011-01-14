using System;
using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core
{
    public interface IUnitReflector
    {
        ITypeReference Get(Type type);
        ITypeReference Get<TReflectionType>();
        ITypeDefinitionExtensions Extend(ITypeReference type);
        ITypeDefinitionExtensions From<TReflectionType>();
    }

    public interface ITypeDefinitionExtensions
    {
        IMethodDefinition GetConstructor(params Type[] arguments);
        IMethodDefinition GetMethod(string name, params Type[] arguments);
        IMethodDefinition GetPropertySetter<TPropertyType>(string propertyName);
        IMethodDefinition GetPropertyGetter(string propertyName);
    }

    public class TypeDefinitionExtensions : ITypeDefinitionExtensions
    {
        private readonly ITypeDefinition type;
        private readonly INameTable nameTable;
        private readonly IUnitReflector reflector;

        public TypeDefinitionExtensions(ITypeDefinition type, INameTable nameTable, IUnitReflector reflector)
        {
            this.type = type;
            this.reflector = reflector;
            this.nameTable = nameTable;
        }

        public IMethodDefinition GetConstructor(params Type[] arguments)
        {
            return GetMethod(".ctor", arguments);
        }

        public IMethodDefinition GetMethod(string name, params Type[] arguments)
        {
            var convertedArguments = new ITypeReference[arguments.Length];
            for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
            {
                convertedArguments[argumentIndex] = reflector.Get(arguments[argumentIndex]);
            }

            return TypeHelper.GetMethod(type, nameTable.GetNameFor(name), convertedArguments);
        }

        public IMethodDefinition GetPropertySetter<TPropertyType>(string propertyName)
        {
            var propertyType = typeof(TPropertyType);
            return GetMethod(String.Format("set_{0}", propertyName), propertyType);
        }

        public IMethodDefinition GetPropertyGetter(string propertyName)
        {
            return GetMethod(String.Format("get_{0}", propertyName));
        }
    }

    public class UnitReflector : IUnitReflector
    {
        private readonly IMetadataHost host;
        private readonly INameTable nameTable;
        private readonly IDictionary<string, ITypeReference> cache = new Dictionary<string, ITypeReference>();

        public UnitReflector(IMetadataHost host)
        {
            this.host = host;
            nameTable = host.NameTable;
        }

        private ITypeReference FindTypeInLoadedUnits(string typeName, int numberOfGenericParameters)
        {
            if (cache.ContainsKey(typeName))
            {
                return cache[typeName];
            }

            foreach (var unit in host.LoadedUnits)
            {
                var foundType = UnitHelper.FindType(nameTable, unit, typeName, numberOfGenericParameters);

                if (foundType.ResolvedType.Equals(Dummy.NamespaceTypeDefinition))
                {
                    continue;
                }

                cache.Add(typeName, foundType);
                return foundType;
            }

            throw new ApplicationException(String.Format("Unable to find type '{0}' in host's loaded units.", typeName));
        }

        public ITypeReference Get(Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                var genericParameters = type.GetGenericArguments();
                var cleanTypeName = type.FullName.Substring(0, type.FullName.IndexOf('`'));
                //return UnitHelper.FindType(nameTable, unit, cleanTypeName, genericParameters.Length);

                return FindTypeInLoadedUnits(cleanTypeName, genericParameters.Length);
            }

            if (type.IsGenericType)
            {
                var genericArguments = type.GetGenericArguments();
                var cleanTypeName = type.FullName.Substring(0, type.FullName.IndexOf('`'));

                var genericType = new GenericTypeInstanceReference();
                var typeDef = Get(type.GetGenericTypeDefinition());
                genericType.GenericType = typeDef;
                //var arguments = new ITypeReference[genericArguments.Length];
                for (int argumentIndex = 0; argumentIndex < genericArguments.Length; argumentIndex++)
                {
                    genericType.GenericArguments.Add(Get(genericArguments[argumentIndex]));
                    //arguments[argumentIndex] = Get(genericArguments[argumentIndex]);
                }

                genericType.PlatformType = host.PlatformType;
                genericType.InternFactory = host.InternFactory;
                genericType.TypeCode = PrimitiveTypeCode.NotPrimitive;

                return genericType;
            }

            return FindTypeInLoadedUnits(type.FullName, 0);
            //return UnitHelper.FindType(nameTable, unit, type.FullName);
        }

        public ITypeReference Get<TReflectionType>()
        {
            return Get(typeof (TReflectionType));
        }

        public ITypeDefinitionExtensions Extend(ITypeReference type)
        {
            return new TypeDefinitionExtensions(type.ResolvedType, nameTable, this);
        }

        public ITypeDefinitionExtensions From<TReflectionType>()
        {
            return Extend(Get(typeof (TReflectionType)));
        }
    }
}
