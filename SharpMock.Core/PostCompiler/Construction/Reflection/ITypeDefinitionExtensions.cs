using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Reflection
{
    public interface ITypeDefinitionExtensions
    {
        IFieldDefinition GetField(string name);
        IMethodDefinition GetConstructor(params Type[] arguments);
        IMethodDefinition GetConstructor(params ITypeReference[] arguments);
        IMethodDefinition GetMethod(string name, params Type[] arguments);
        IMethodDefinition GetMethod(IMethodReference method);
        IMethodDefinition GetMethod(MethodInfo methodInfo);
        IEnumerable<IMethodDefinition> GetAllOverloadsOf(string name);
        IMethodDefinition GetPropertySetter<TPropertyType>(string propertyName);
        IMethodDefinition GetPropertySetter(string propertyName, Type propertyType);
        IMethodDefinition GetPropertyGetter(string propertyName);
    }
}