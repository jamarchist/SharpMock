using System;
using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface ITypeDefinitionExtensions
    {
        IMethodDefinition GetConstructor(params Type[] arguments);
        IMethodDefinition GetMethod(string name, params Type[] arguments);
        IMethodDefinition GetMethod(IMethodReference method);
        IMethodDefinition GetPropertySetter<TPropertyType>(string propertyName);
        IMethodDefinition GetPropertySetter(string propertyName, Type propertyType);
        IMethodDefinition GetPropertyGetter(string propertyName);
    }
}