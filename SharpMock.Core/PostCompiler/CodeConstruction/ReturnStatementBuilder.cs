using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
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
}
