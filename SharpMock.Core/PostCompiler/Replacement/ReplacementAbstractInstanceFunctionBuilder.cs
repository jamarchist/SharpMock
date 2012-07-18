using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Utility;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementAbstractInstanceFunctionBuilder : ReplacementMethodBuilderBase
    {
        public ReplacementAbstractInstanceFunctionBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
        }

        protected override void BuildMethodTemplate()
        {
            Context.Log.WriteTrace(String.Empty);
            Context.Log.WriteTrace("BuildingMethod: {0}.", Context.OriginalCall.Name.Value);

            AddStatement.DeclareRegistryInterceptor();
            AddStatement.DeclareInvocation();
            AddStatement.DeclareInterceptedType(Context.OriginalCall.ContainingType.ResolvedType);
            AddStatement.DeclareParameterTypesArray(Context.OriginalCall.ParameterCount);
            AddStatement.DeclareArgumentsList();

            foreach (var parameter in Context.OriginalCall.Parameters)
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
                                   , Context.OriginalCall.Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable<MethodInfo>("interceptedMethod").As(
                    Call.VirtualMethod("GetMethod", typeof(string), typeof(Type[]))
                        .ThatReturns<MethodInfo>()
                        .WithArguments(
                            Constant.Of(Context.OriginalCall.Name.Value),
                            Locals["parameterTypes"])
                        .On("interceptedType"))
                );

            var parameterTypes = Context.OriginalCall.Parameters.Select(p => p.Type);
            var parameterTypesWithReturnType = new List<ITypeReference>(parameterTypes);
            parameterTypesWithReturnType.Add(Context.OriginalCall.Type);

            var anonymousMethod = Anon.Func(parameterTypesWithReturnType.ToArray())
                .WithBody(c =>
                              {
                                  c.AddLine(x =>
                                                {
                                                    var parameters = x.Params.ToList();
                                                    var target = Params["target"];
                                                    var originalMethodCall = x.Call.VirtualMethod(Context.OriginalCall)
                                                        .ThatReturns(Context.OriginalCall.Type)
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