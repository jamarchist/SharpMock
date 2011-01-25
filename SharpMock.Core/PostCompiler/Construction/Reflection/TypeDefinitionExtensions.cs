﻿using System;
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