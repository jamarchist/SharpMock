using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IInstanceCreator
    {
        CreateObjectInstance New<TReflectionType>();
    }
}
