using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public interface IInstanceCreator
    {
        IInstanceCreatorOptions New(ITypeReference type, params ITypeReference[] constructorParameters);
        CreateObjectInstance New<TReflectionType>();
        DefaultValue Default<TReflectionType>();
        CreateArray NewArray<TReflectionType>(int size);
    }
}
