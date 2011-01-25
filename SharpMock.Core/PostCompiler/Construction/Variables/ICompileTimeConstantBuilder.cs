using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public interface ICompileTimeConstantBuilder
    {
        CompileTimeConstant Of<TConstantType>(TConstantType value);
    }
}