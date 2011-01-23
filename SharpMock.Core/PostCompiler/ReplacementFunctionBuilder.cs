using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.DelegateTypes;

namespace SharpMock.PostCompiler.Core
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
            var returnGetter = Reflector.From<Invocation>().GetPropertyGetter("Return");

            var getReturn = new MethodCall();
            getReturn.MethodToCall = returnGetter;
            getReturn.Type = Context.Host.PlatformType.SystemObject;
            getReturn.ThisArgument = Locals["invocation"];

            var castResultOfGetReturn = new Conversion();
            castResultOfGetReturn.ValueToConvert = getReturn;
            castResultOfGetReturn.TypeAfterConversion = Context.FakeMethod.Type;

            //Declare.Variable("interceptionResult", fakeMethod.Type).As(castResultOfGetReturn);
            var interceptionResultDeclaration =
                Declare.Variable("interceptionResult", Context.FakeMethod.Type).As(castResultOfGetReturn);

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
            var originalMethodCallDeclaration = new LocalDeclarationStatement();
            var originalMethodReturnValueDefinition = new LocalDefinition();
            originalMethodReturnValueDefinition.Type = originalMethodCall.Type;
            originalMethodReturnValueDefinition.Name = Context.Host.NameTable.GetNameFor("originalMethodCallReturnValue");

            originalMethodCallDeclaration.LocalVariable = originalMethodReturnValueDefinition;
            originalMethodCallDeclaration.InitialValue = originalMethodCall;

            anonymousMethodBody.Statements.Add(originalMethodCallDeclaration);

            var @return = new BoundExpression();
            @return.Definition = originalMethodReturnValueDefinition;
            @return.Type = originalMethodCall.Type;
            anonymousMethodReturnStatement.Expression = @return;
        }
    }
}