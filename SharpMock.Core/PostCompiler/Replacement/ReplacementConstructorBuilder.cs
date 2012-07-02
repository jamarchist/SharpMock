using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementConstructorBuilder : ReplacementFunctionBuilder
    {
        public ReplacementConstructorBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
        }

        protected override void AddOriginalMethodCallStatement(Microsoft.Cci.MutableCodeModel.BlockStatement anonymousMethodBody, Microsoft.Cci.MutableCodeModel.ReturnStatement anonymousMethodReturnStatement, Microsoft.Cci.MutableCodeModel.MethodCall originalMethodCall)
        {
            var parameterTypes = new List<ITypeReference>();
            foreach (var p in originalMethodCall.MethodToCall.Parameters)
            {
                parameterTypes.Add(p.Type);
            }

            var newDeclaration = Declare.Variable("originalConstructorResult", Context.FakeMethod.Type)
                .As(Create.New(Context.FakeMethod.Type, parameterTypes.ToArray()));

            anonymousMethodBody.Statements.Add(newDeclaration);
            anonymousMethodReturnStatement.Expression = Locals["originalConstructorResult"];
        }
    }
}