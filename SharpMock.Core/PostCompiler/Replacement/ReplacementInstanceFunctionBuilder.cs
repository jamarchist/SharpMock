using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Utility;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementInstanceFunctionBuilder : ReplacementMethodBuilderBase
    {
        private readonly IMethodReference instanceFunction;

        public ReplacementInstanceFunctionBuilder(ReplacementMethodConstructionContext context, IMethodReference instanceFunction) : base(context)
        {
            this.instanceFunction = instanceFunction;
        }

        public override void BuildMethod()
        {
            Context.Log.WriteTrace(String.Empty);
            Context.Log.WriteTrace("BuildingMethod: {0}.", instanceFunction.Name.Value);

            AddStatement.DeclareRegistryInterceptor();
            AddStatement.DeclareInvocation();
            AddStatement.DeclareInterceptedType(instanceFunction.ContainingType.ResolvedType);
            AddStatement.DeclareParameterTypesArray(instanceFunction.ParameterCount);
            AddStatement.DeclareArgumentsList();

            foreach (var parameter in instanceFunction.Parameters)
            {
                AddStatement.AssignParameterTypeValue(parameter.Index, parameter.Type.ResolvedType);
            }

            foreach (var parameter in Params.ToList())
            {
                if ((parameter.Definition as IParameterDefinition).Name.Value != "target")
                {
                    AddStatement.AddArgumentToList(parameter);
                }
            }

            Context.Log.WriteTrace("  Adding: var interceptedMethod = interceptedType.GetMethod('{0}', parameterTypes);"
                , instanceFunction.Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable<MethodInfo>("interceptedMethod").As(
                Call.VirtualMethod("GetMethod", typeof(string), typeof(Type[]))
                    .ThatReturns<MethodInfo>()
                    .WithArguments(
                        Constant.Of(instanceFunction.Name.Value),
                        Locals["parameterTypes"])
                    .On("interceptedType"))
            );

            var parameterTypes = instanceFunction.Parameters.Select(p => p.Type);
            var parameterTypesWithReturnType = new List<ITypeReference>(parameterTypes);
            parameterTypesWithReturnType.Add(instanceFunction.Type);

            var anonymousMethod = Anon.Func(parameterTypesWithReturnType.ToArray())
                        .WithBody(c =>
                        {
                            c.AddLine(x =>
                            {
                                var parameters = x.Params.ToList();
                                var target = Params["target"];
                                var originalMethodCall = x.Call.Method(instanceFunction)
                                                                .ThatReturns(instanceFunction.Type)
                                                                .WithArguments(parameters.Select(p => p as IExpression).ToArray())
                                                                .On(target);
                                return x.Declare.Variable("anonReturn", originalMethodCall.Type).As(originalMethodCall);
                            });
                            c.AddLine(x => x.Return.Variable(x.Locals["anonReturn"]));
                        });
            Context.Block.Statements.Add(
                Declare.Variable("local_0", anonymousMethod.Type).As(anonymousMethod)
            );

            AddStatement.CallShouldInterceptOnInterceptor();
            AddStatement.SetOriginalCallOnInvocation();
            AddStatement.SetArgumentsOnInvocation();
            AddStatement.SetTargetOnInvocationToTargetParameter();
            AddStatement.SetOriginalCallInfoOnInvocation();
            AddStatement.CallInterceptOnInterceptor();

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