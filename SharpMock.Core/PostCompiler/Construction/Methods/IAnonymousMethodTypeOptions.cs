using System;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface IAnonymousMethodTypeOptions
    {
        IAnonymousMethodBodyBuilder Of<TDelegateType>();
        IAnonymousMethodBodyBuilder Func(params ITypeReference[] typeParameters);
        IAnonymousMethodBodyBuilder Action(params ITypeReference[] typeParameters);
    }
}