using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.ControlFlow
{
    public class IfStatementOptions : IIfStatementOptions
    {
        private readonly ConditionalStatement ifStatement;

        public IfStatementOptions(ConditionalStatement ifStatement)
        {
            this.ifStatement = ifStatement;
        }

        public IIfBranchOptions Then(VoidAction<List<IStatement>> code)
        {
            var blockStatements = new List<IStatement>();
            code(blockStatements);

            var trueBranch = new BlockStatement();
            trueBranch.Statements = blockStatements;
            
            return new IfBranchOptions(ifStatement);
        }
    }
}