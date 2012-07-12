using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementFunctionBuilder : ReplacementMethodBuilder
    {
        public ReplacementFunctionBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
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

        protected override void AddReturnStatement()
        {
            Context.Log.WriteTrace("  Adding: var interceptionResult = ({0})invocation.Return;",
                (Context.FakeMethod.Type.ResolvedType as INamedEntity).Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable("interceptionResult", Context.FakeMethod.Type).As(
                    ChangeType.Convert(Call.PropertyGetter<object>("Return").On("invocation")).To(Context.FakeMethod.Type))
            );

            Context.Log.WriteTrace("  Adding: return interceptionResult;");
            Context.Block.Statements.Add(
                Return.Variable(Locals["interceptionResult"])
            );
        }
    }
}