using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Declarations;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementConstructorBuilder : ReplacementFunctionBuilder
    {
        public ReplacementConstructorBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
        }

        protected override void AddOriginalMethodCallStatement(BlockStatement anonymousMethodBody, ReturnStatement anonymousMethodReturnStatement, MethodCall originalMethodCall)
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

        protected override MethodCall CallGetMethodInfoMethod()
        {
            return Call.VirtualMethod("GetConstructor", typeof(Type[]))
                    .ThatReturns<ConstructorInfo>()
                    .WithArguments(
                        Locals["parameterTypes"])
                    .On("interceptedType");
        }

        protected override IDynamicDeclarationOptions DeclareMethodInfoVariable()
        {
            return Declare.Variable("interceptedMethod", Reflector.Get<ConstructorInfo>());
        }
    }
}