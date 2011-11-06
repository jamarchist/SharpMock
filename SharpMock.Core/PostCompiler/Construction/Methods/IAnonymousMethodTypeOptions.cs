using System;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface IAnonymousMethodTypeOptions
    {
        IAnonymousMethodBodyBuilder Of<TDelegateType>();
    }
}