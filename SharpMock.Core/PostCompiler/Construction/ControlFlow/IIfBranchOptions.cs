using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.ControlFlow
{
    public interface IIfBranchOptions
    {
        ConditionalStatement EndIf();
        ConditionalStatement Else(VoidAction<List<IStatement>> code);
    }
}