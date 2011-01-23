using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public class ReplacementFunctionBuilder : ReplacementMethodBuilder
    {
        public ReplacementFunctionBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
        }

        protected override void AddInterceptionExtraResultHandling(LocalDeclarationStatement interceptionResultDeclaration)
        {
            Context.Block.Statements.Add(interceptionResultDeclaration);
        }

        protected override LocalDeclarationStatement AddInterceptionResultHandling(ReturnStatement returnStatement)
        {
            // ...
            // var interceptionResult = (METHOD_RETURN_TYPE)invocation.Return;
            // ...
            var interceptionResultDeclaration = Declare.Variable<object>("interceptionResult").As(
                ChangeType.Convert(Call.PropertyGetter<object>("Return").On("invocation")).To(Context.FakeMethod.Type));

            returnStatement.Expression = Locals["interceptionResult"];
            return interceptionResultDeclaration;
        }

        protected override void AddReturnTypeSpecificGenericArguments(GenericTypeInstanceReference closedGenericFunction)
        {
            closedGenericFunction.GenericArguments.Add(Context.FakeMethod.Type);
        }

        protected override ITypeReference GetOpenGenericFunction()
        {
            return SharpMockTypes.Functions[Context.OriginalCall.ParameterCount];
        }

        protected override void AddOriginalMethodCallStatement(
            BlockStatement anonymousMethodBody, ReturnStatement anonymousMethodReturnStatement, MethodCall originalMethodCall)
        {
            var originalCall =
                Declare.Variable("originalCallReturnValue", Context.OriginalCall.Type).As(originalMethodCall);

            anonymousMethodBody.Statements.Add(originalCall);
            anonymousMethodReturnStatement.Expression = Locals["originalCallReturnValue"];
        }
    }
}