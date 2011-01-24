using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public interface IInstanceCreator
    {
        CreateObjectInstance New<TReflectionType>();
        CreateArray NewArray<TReflectionType>(int size);
    }
}
