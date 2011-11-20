using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Construction
{
    public class ReturnStatementBuilder : IReturnStatementBuilder
    {
        private readonly BoundExpression variableBinding = new BoundExpression();
        private readonly ReturnStatement @return = new ReturnStatement();

        public IReturnStatementBuilder Variable(ILocalDefinition localVariable)
        {
            @return.Expression = variableBinding;
            variableBinding.Definition = localVariable;
            return this;
        }

        public IReturnStatementBuilder OfType(ITypeReference type)
        {
            variableBinding.Type = type;
            return this;
        }

        public void In(BlockStatement block)
        {
            block.Statements.Add(@return);
        }

        public IReturnStatementBuilder NullOrVoid()
        {
            @return.Expression = new CompileTimeConstant();
            return this;
        }
    }

    public interface ICodeReturnStatementBuilder
    {
        IStatement Variable(IBoundExpression localVariable);
        IStatement Null();
        IStatement Void();
    }

    public class CodeReturnStatementBuilder : ICodeReturnStatementBuilder
    {
        private readonly ReturnStatement @return = new ReturnStatement();

        public IStatement Variable(IBoundExpression localVariable)
        {
            @return.Expression = localVariable;
            return @return;
        }

        public IStatement Null()
        {
            throw new System.NotImplementedException();
        }

        public IStatement Void()
        {
            return @return;
        }
    }
}
