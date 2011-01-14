using System;
using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
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