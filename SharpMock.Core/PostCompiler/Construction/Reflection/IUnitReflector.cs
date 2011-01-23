using System;
using Microsoft.Cci;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction.Reflection
{
    public interface IUnitReflector
    {
        ITypeReference Get(Type type);
        ITypeReference Get<TReflectionType>();
        ITypeDefinitionExtensions Extend(ITypeReference type);
        ITypeDefinitionExtensions From<TReflectionType>();
        ITypeDefinitionExtensions From(ITypeReference type);
    }
}