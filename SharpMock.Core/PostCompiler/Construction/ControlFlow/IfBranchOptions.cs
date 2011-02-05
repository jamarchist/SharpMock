using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.ControlFlow
{
    public class IfBranchOptions : IIfBranchOptions
    {
        private readonly ConditionalStatement ifStatement;

        public IfBranchOptions(ConditionalStatement ifStatement)
        {
            this.ifStatement = ifStatement;
        }

        public ConditionalStatement EndIf()
        {
            return ifStatement;
        }

        public ConditionalStatement Else(VoidAction<List<IStatement>> code)
        {
            var blockStatements = new List<IStatement>();
            code(blockStatements);

            var elseBranch = new BlockStatement();
            elseBranch.Statements = blockStatements;

            ifStatement.FalseBranch = elseBranch;
            return ifStatement;
        }
    }
}