using System;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction
{
    public interface ITypeOperatorBuilder
    {
        IExpression TypeOf(ITypeReference typeReference);
    }
}
