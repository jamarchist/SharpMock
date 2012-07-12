using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Utility;

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

        protected override void AddAnonymousMethodDeclaration()
        {
            var parameterTypes = Context.OriginalCall.Parameters.Select(p => p.Type);
            var parameterCounter = 0;

            Context.Log.WriteTrace("  Adding: VoidAction<{0}> local_0 = ({1}) => {{", 
                parameterTypes.Select(p => (p as INamedTypeReference).Name.Value).CommaDelimitedList(), 
                parameterTypes.Select(p => String.Format("alteredp{0}", parameterCounter++)).CommaDelimitedList());
            var anonymousMethod = Anon.Action(parameterTypes.ToArray())
                        .WithBody(c => c.AddLine(x =>
                                                {
                                                    var parameters = x.Params.ToList();

                                                    MethodCall originalMethodCall = null;
                                                    if (Context.OriginalCall.ResolvedMethod.IsStatic || Context.OriginalCall.ResolvedMethod.IsConstructor)
                                                    {
                                                        originalMethodCall = Call.StaticMethod(Context.OriginalCall)
                                                                                    .ThatReturnsVoid()
                                                                                    .WithArguments(parameters.Select(p => p as IExpression).ToArray())
                                                                                    .On(Context.OriginalCall.ResolvedMethod.ContainingTypeDefinition);
                                                    }
                                                    else
                                                    {
                                                        var target = Params["target"];

                                                        if (Context.OriginalCall.ContainingType.ResolvedType.IsInterface || Context.OriginalCall.ContainingType.ResolvedType.IsAbstract)
                                                        {
                                                            originalMethodCall = Call.VirtualMethod(Context.OriginalCall)
                                                                                    .ThatReturnsVoid()
                                                                                    .WithArguments(parameters.Select(p => p as IExpression).ToArray())
                                                                                    .On(target);
                                                        }
                                                        else
                                                        {
                                                            Context.Log.WriteTrace("            target.{0}({1});", Context.OriginalCall.Name.Value,
                                                                parameters.Select(p => p.Definition as IParameterDefinition).Select(p => p.Name.Value).CommaDelimitedList());
                                                            originalMethodCall = Call.Method(Context.OriginalCall)
                                                                                    .ThatReturnsVoid()
                                                                                    .WithArguments(parameters.Select(p => p as IExpression).ToArray())
                                                                                    .On(target);
                                                        }
                                                    }

                                                    return x.Do(originalMethodCall);
                                                }));
            Context.Log.WriteTrace("          };");
            Context.Block.Statements.Add(
                Declare.Variable("local_0", anonymousMethod.Type).As(anonymousMethod)
            );
        }
    }
}