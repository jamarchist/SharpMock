using System;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Reflection
{
    public interface IUnitReflector
    {
        ITypeReference Get(string fullyQualifiedName);
        ITypeReference Get(Type type);
        ITypeReference Get<TReflectionType>();
        ITypeReference GetGeneric(Type genericType, Type[] typeParameters);
        ITypeDefinitionExtensions Extend(ITypeReference type);
        ITypeDefinitionExtensions From<TReflectionType>();
        ITypeDefinitionExtensions From(string fullyQualifiedName);
        ITypeDefinitionExtensions From(Type reflectionType);
        ITypeDefinitionExtensions From(ITypeReference type);
    }
}