using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Utility;

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

        protected override void AddAnonymousMethodDeclaration()
        {
            var parameterTypes = Context.OriginalCall.Parameters.Select(p => p.Type);
            var parameterTypesWithReturnType = new List<ITypeReference>(parameterTypes);
            parameterTypesWithReturnType.Add(Context.OriginalCall.Type);

            var anonymousMethod = Anon.Func(parameterTypesWithReturnType.ToArray())
                        .WithBody(c =>
                                    {
                                        c.AddLine(x =>
                                        {
                                            var parameters = x.Params.ToList();

                                            MethodCall originalMethodCall = null;
                                            if (Context.OriginalCall.ResolvedMethod.IsStatic || Context.OriginalCall.ResolvedMethod.IsConstructor)
                                            {
                                                originalMethodCall = x.Call.StaticMethod(Context.OriginalCall)
                                                                            .ThatReturns(Context.OriginalCall.Type)
                                                                            .WithArguments(parameters.Select(p => p as IExpression).ToArray())
                                                                            .On(Context.OriginalCall.ResolvedMethod.ContainingTypeDefinition);
                                            }
                                            else
                                            {
                                                var target = Params["target"];

                                                if (Context.OriginalCall.ContainingType.ResolvedType.IsInterface || Context.OriginalCall.ContainingType.ResolvedType.IsAbstract)
                                                {
                                                    originalMethodCall = x.Call.VirtualMethod(Context.OriginalCall)
                                                                            .ThatReturns(Context.OriginalCall.Type)
                                                                            .WithArguments(parameters.Select(p => p as IExpression).ToArray())
                                                                            .On(target);
                                                }
                                                else
                                                {
                                                    originalMethodCall = x.Call.Method(Context.OriginalCall)
                                                                            .ThatReturns(Context.OriginalCall.Type)
                                                                            .WithArguments(parameters.Select(p => p as IExpression).ToArray())
                                                                            .On(target);
                                                }
                                            }

                                            return x.Declare.Variable("anonReturn", originalMethodCall.Type).As(originalMethodCall);
                                        });
                                        c.AddLine(x => x.Return.Variable(x.Locals["anonReturn"]));
                                    });
            Context.Block.Statements.Add(
                Declare.Variable("local_0", anonymousMethod.Type).As(anonymousMethod)               
            );
        }
    }
}