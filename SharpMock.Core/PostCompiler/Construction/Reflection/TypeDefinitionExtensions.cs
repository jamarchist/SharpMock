using System;
using System.Collections.Generic;
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

            var method = TypeHelper.GetMethod(type, nameTable.GetNameFor(name), convertedArguments);
            
            // Try to find the method by brute force
            if (method.Equals(Dummy.Method))
            {
                foreach (var member in type.Methods)
                {
                    if (member.Name.Value == name)
                    {
                        var parameters = new List<IParameterDefinition>(member.Parameters);

                        if (parameters.Count == convertedArguments.Length)
                        {
                            var matches = new bool[arguments.Length];

                            for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                            {
                                matches[parameterIndex] = TypeHelper.TypesAreEquivalent(
                                    convertedArguments[parameterIndex], parameters[parameterIndex].Type);
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

        public IMethodDefinition GetMethod(IMethodReference method)
        {
            return TypeHelper.GetMethod(type, method);
        }

        public IEnumerable<IMethodDefinition> GetAllOverloadsOf(string name)
        {
            var methods = new List<IMethodDefinition>();
            foreach (var method in type.Methods)
            {
                if (method.Name.Value == name)
                {
                    methods.Add(method);   
                }
            }

            return methods;
        }

        //public IMethodDefinition GetGenericMethod(string name, params Type[] parameters)
        //{
        //    var convertedArguments = new ITypeReference[parameters.Length];
        //    for (int argumentIndex = 0; argumentIndex < parameters.Length; argumentIndex++)
        //    {
        //        convertedArguments[argumentIndex] = reflector.Get(parameters[argumentIndex]);
        //    }

        //    foreach (var method in type.Methods)
        //    {
        //        if (method.IsGeneric && method.Name.Value == name)
        //        {
        //            var possibleMethodParameters = new List<IParameterDefinition>(method.Parameters);

        //            if (possibleMethodParameters.Count == convertedArguments.Length)
        //            {
        //                var matches = new bool[parameters.Length];

        //                for (int parameterIndex = 0; parameterIndex < possibleMethodParameters.Count; parameterIndex++)
        //                {
        //                    var parameterOfMethodToGet = convertedArguments[parameterIndex];
        //                    var parameterOfExaminedMethod = possibleMethodParameters[parameterIndex].Type;

        //                    var nonGenericTypesMatch = TypeHelper.TypesAreEquivalent(parameterOfMethodToGet, parameterOfExaminedMethod);
        //                    var genericTypesMatch = false;

        //                    if (parameterOfMethodToGet.ResolvedType.IsGeneric && parameterOfExaminedMethod.ResolvedType.IsGeneric)
        //                    {
        //                        genericTypesMatch = TypeHelper.
        //                            TypesAreEquivalentAssumingGenericMethodParametersAreEquivalentIfTheirIndicesMatch(
        //                                parameterOfExaminedMethod.ResolvedType,
        //                                parameterOfMethodToGet.ResolvedType.InstanceType);
        //                    }

        //                    matches[parameterIndex] = nonGenericTypesMatch || genericTypesMatch;
        //                }

        //                var matchList = new List<bool>(matches);
        //                if (matchList.Contains(false))
        //                {
        //                    continue;
        //                }
        //                return method;
        //            }
        //        }
        //    }

        //    return Dummy.Method;
        //}

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
            return GetMethod(String.Format("get_{0}", propertyName));
        }
    }
}