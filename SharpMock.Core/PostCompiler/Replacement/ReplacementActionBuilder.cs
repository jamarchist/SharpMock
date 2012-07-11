using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementActionBuilder : ReplacementMethodBuilder
    {
        public ReplacementActionBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
        }

        protected override void AddReturnTypeSpecificGenericArguments(GenericTypeInstanceReference closedGenericFunction)
        {
            // Do nothing
        }

        protected override ITypeReference GetOpenGenericFunction()
        {
            return SharpMockTypes.Actions[Context.OriginalCall.ParameterCount];
        }

        protected override void AddOriginalMethodCallStatement(BlockStatement anonymousMethodBody, ReturnStatement anonymousMethodReturnStatement, MethodCall originalMethodCall)
        {
            var originalMethodCallStatement = new ExpressionStatement();
            originalMethodCallStatement.Expression = originalMethodCall;

            anonymousMethodBody.Statements.Add(originalMethodCallStatement);
        }

        protected override void AddReturnStatement()
        {
            Context.Block.Statements.Add(Return.Void());
        }
    }
}