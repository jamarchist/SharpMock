using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Utility;

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

        protected override void AddInterceptedMethodDeclaration()
        {
            Context.Log.WriteTrace("  Adding: var interceptedMethod = interceptedType.GetConstructor(parameterTypes);");
            Context.Block.Statements.Add(
                Declare.Variable<ConstructorInfo>("interceptedMethod").As(
                Call.VirtualMethod("GetConstructor", typeof(Type[]))
                    .ThatReturns<ConstructorInfo>()
                    .WithArguments(
                        Locals["parameterTypes"])
                    .On("interceptedType"))
            );
        }

        protected override void AddAnonymousMethodDeclaration()
        {
            var parameterTypes = Context.OriginalCall.Parameters.Select(p => p.Type);
            var parameterTypesWithReturnType = new List<ITypeReference>(parameterTypes);
            parameterTypesWithReturnType.Add(Context.FakeMethod.Type);

            var anonymousMethod = Anon.Func(parameterTypesWithReturnType.ToArray())
                .WithBody(c =>
                              {
                                  c.AddLine(x => x.Declare.Variable("originalConstructorResult", Context.FakeMethod.Type)
                                    .As(x.Create.New(Context.FakeMethod.Type, parameterTypes.ToArray())));
                                  c.AddLine(x => x.Return.Variable(x.Locals["originalConstructorResult"]));
                              });
            Context.Block.Statements.Add(
                Declare.Variable("local_0", anonymousMethod.Type).As(anonymousMethod)
            );
        }
    }
}