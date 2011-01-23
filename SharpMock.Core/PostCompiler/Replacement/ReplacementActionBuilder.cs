using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementActionBuilder : ReplacementMethodBuilder
    {
        public ReplacementActionBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
        }

        protected override void AddInterceptionExtraResultHandling(LocalDeclarationStatement interceptionResultDeclaration)
        {
            // Do nothing
        }

        protected override LocalDeclarationStatement AddInterceptionResultHandling(ReturnStatement returnStatement)
        {
            return new LocalDeclarationStatement();
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
    }
}