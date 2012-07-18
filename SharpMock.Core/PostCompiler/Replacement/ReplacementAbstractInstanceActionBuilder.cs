using System;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Utility;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementAbstractInstanceActionBuilder : ReplacementMethodBuilderBase
    {
        private readonly IMethodReference abstractInstanceAction;

        public ReplacementAbstractInstanceActionBuilder(ReplacementMethodConstructionContext context, IMethodReference abstractInstanceAction) : base(context)
        {
            this.abstractInstanceAction = abstractInstanceAction;
        }

        public override void BuildMethod()
        {
            Context.Log.WriteTrace(String.Empty);
            Context.Log.WriteTrace("BuildingMethod: {0}.", abstractInstanceAction.Name.Value);

            AddStatement.DeclareRegistryInterceptor();
            AddStatement.DeclareInvocation();
            AddStatement.DeclareInterceptedType(abstractInstanceAction.ContainingType.ResolvedType);
            AddStatement.DeclareParameterTypesArray(abstractInstanceAction.ParameterCount);
            AddStatement.DeclareArgumentsList();

            foreach (var parameter in abstractInstanceAction.Parameters)
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
                                   , abstractInstanceAction.Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable<MethodInfo>("interceptedMethod").As(
                    Call.VirtualMethod("GetMethod", typeof(string), typeof(Type[]))
                        .ThatReturns<MethodInfo>()
                        .WithArguments(
                            Constant.Of(abstractInstanceAction.Name.Value),
                            Locals["parameterTypes"])
                        .On("interceptedType"))
                );

            var parameterTypes = abstractInstanceAction.Parameters.Select(p => p.Type);
            var parameterCounter = 0;

            Context.Log.WriteTrace("  Adding: VoidAction<{0}> local_0 = ({1}) => {{",
                                   parameterTypes.Select(p => (p as INamedTypeReference).Name.Value).CommaDelimitedList(),
                                   parameterTypes.Select(p => String.Format("alteredp{0}", parameterCounter++)).CommaDelimitedList());
            var anonymousMethod = Anon.Action(parameterTypes.ToArray())
                .WithBody(c => c.AddLine(x =>
                                             {
                                                 var parameters = x.Params.ToList();
                                                 var target = Params["target"];

                                                 var originalMethodCall = Call.VirtualMethod(abstractInstanceAction)
                                                     .ThatReturnsVoid()
                                                     .WithArguments(parameters.Select(p => p as IExpression).ToArray())
                                                     .On(target);
                                                 return x.Do(originalMethodCall);
                                             }));
            Context.Log.WriteTrace("          };");
            Context.Block.Statements.Add(
                Declare.Variable("local_0", anonymousMethod.Type).As(anonymousMethod)
                );

            AddStatement.CallShouldInterceptOnInterceptor();
            AddStatement.SetOriginalCallOnInvocation();
            AddStatement.SetArgumentsOnInvocation();
            AddStatement.SetTargetOnInvocationToTargetParameter();

            AddStatement.SetOriginalCallInfoOnInvocation();
            AddStatement.CallInterceptOnInterceptor();

            Context.Block.Statements.Add(Return.Void());
        }
    }
}