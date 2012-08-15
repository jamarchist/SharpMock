using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Reflection
{
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

        public IFieldDefinition GetField(string name)
        {
            return TypeHelper.GetField(type, nameTable.GetNameFor(name));
        }

        public IMethodDefinition GetConstructor(params Type[] arguments)
        {
            return GetMethod(".ctor", arguments);
        }

        public IMethodDefinition GetConstructor(params ITypeReference[] arguments)
        {
            return GetMethod(".ctor", arguments);
        }

        public IMethodDefinition GetMethod(string name, params ITypeReference[] arguments)
        {
            var method = TypeHelper.GetMethod(type, nameTable.GetNameFor(name), arguments);

            // Try to find the method by brute force
            if (method.Equals(Dummy.Method))
            {
                foreach (var member in type.Methods)
                {
                    if (member.Name.Value == name)
                    {
                        var parameters = new List<IParameterDefinition>(member.Parameters);

                        if (parameters.Count == arguments.Length)
                        {
                            var matches = new bool[arguments.Length];

                            for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                            {
                                matches[parameterIndex] = TypeHelper.TypesAreEquivalent(
                                    arguments[parameterIndex], parameters[parameterIndex].Type);
                            }

                            var matchList = new List<bool>(matches);
                            if (matchList.Contains(false))
                            {
                                continue;
                            }
                            else
                            {
                                method = member;
                                break;
                            }
                        }
                    }
                }
            }

            return method;            
        }

        public IMethodDefinition GetMethod(string name, params Type[] arguments)
        {
            var convertedArguments = new ITypeReference[arguments.Length];
            for (int argumentIndex = 0; argumentIndex < arguments.Length; argumentIndex++)
            {
                convertedArguments[argumentIndex] = reflector.Get(arguments[argumentIndex]);
            }

            return GetMethod(name, convertedArguments);
        }

        public IMethodDefinition GetMethod(MethodInfo methodInfo)
        {
            var name = methodInfo.Name;
            var parameters = methodInfo.GetParameters();
            var arguments = new Type[parameters.Length];

            for (int index = 0; index < parameters.Length; index++)
            {
                arguments[index] = parameters[index].ParameterType;
            }

            return GetMethod(name, arguments);
        }

        public IMethodDefinition GetMethod(IMethodReference method)
        {
            return TypeHelper.GetMethod(type, method);
        }

        public IEnumerable<IMethodDefinition> GetAllOverloadsOf(string name)
        {
            return GetAllOverloadsOf(name, TypeMemberVisibility.Public);
        }

        public IEnumerable<IMethodDefinition> GetAllOverloadsOf(string name, TypeMemberVisibility visibility)
        {
            var methods = new List<IMethodDefinition>();
            foreach (var method in type.Methods)
            {
                if (method.Name.Value == name && method.Visibility == visibility)
                {
                    methods.Add(method);
                }
            }

            return methods;            
        }

        public IMethodDefinition GetPropertySetter<TPropertyType>(string propertyName)
        {
            var propertyType = typeof(TPropertyType);
            return GetPropertySetter(propertyName, propertyType);
        }

        public IMethodDefinition GetPropertySetter(string propertyName, Type propertyType)
        {
            return GetMethod(String.Format("set_{0}", propertyName), propertyType);            
        }

        public IMethodDefinition GetPropertyGetter(string propertyName)
        {
            return GetMethod(String.Format("get_{0}", propertyName), Type.EmptyTypes);
        }
    }
}