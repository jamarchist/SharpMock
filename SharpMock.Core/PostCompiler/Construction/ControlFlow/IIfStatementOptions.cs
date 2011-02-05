using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.ControlFlow
{
    public interface IIfStatementOptions
    {
        IIfBranchOptions Then(VoidAction<List<IStatement>> code);
    }
}