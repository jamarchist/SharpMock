﻿using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.PostCompiler.Core.CodeConstruction;

namespace SharpMock.Core.PostCompiler.Replacement
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
            // [ In order to produce the correct IL, we need to declare the correct variable type. 'Object' doesn't cut it. ]
            var interceptionResultDeclaration = Declare.Variable("interceptionResult", Context.FakeMethod.Type).As(
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