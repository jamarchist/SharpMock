using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Reflection
{
    public interface ITypeDefinitionExtensions
    {
        IMethodDefinition GetConstructor(params Type[] arguments);
        IMethodDefinition GetMethod(string name, params Type[] arguments);
        IMethodDefinition GetMethod(IMethodReference method);
        IMethodDefinition GetMethod(MethodInfo methodInfo);
        //IMethodDefinition GetGenericMethod(string name, params Type[] parameters);
        IEnumerable<IMethodDefinition> GetAllOverloadsOf(string name);
        IMethodDefinition GetPropertySetter<TPropertyType>(string propertyName);
        IMethodDefinition GetPropertySetter(string propertyName, Type propertyType);
        IMethodDefinition GetPropertyGetter(string propertyName);
    }
}