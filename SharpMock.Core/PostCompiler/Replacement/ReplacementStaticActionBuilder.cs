using System;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Utility;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementStaticActionBuilder : ReplacementMethodBuilderBase
    {
        private readonly IMethodReference staticAction;

        public ReplacementStaticActionBuilder(ReplacementMethodConstructionContext context, IMethodReference staticAction) : base(context)
        {
            this.staticAction = staticAction;
        }

        public override void BuildMethod()
        {
            Context.Log.WriteTrace(String.Empty);
            Context.Log.WriteTrace("BuildingMethod: {0}.", staticAction.Name.Value);

            AddStatement.DeclareRegistryInterceptor();
            AddStatement.DeclareInvocation();
            AddStatement.DeclareInterceptedType(staticAction.ContainingType.ResolvedType);
            AddStatement.DeclareParameterTypesArray(staticAction.ParameterCount);
            AddStatement.DeclareArgumentsList();

            foreach (var parameter in staticAction.Parameters)
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
                                   , staticAction.Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable<MethodInfo>("interceptedMethod").As(
                    Call.VirtualMethod("GetMethod", typeof(string), typeof(Type[]))
                        .ThatReturns<MethodInfo>()
                        .WithArguments(
                            Constant.Of(staticAction.Name.Value),
                            Locals["parameterTypes"])
                        .On("interceptedType"))
                );

            var parameterTypes = staticAction.Parameters.Select(p => p.Type);

            //var parameterCounter = 0;
            //Context.Log.WriteTrace("  Adding: VoidAction<{0}> local_0 = ({1}) => {{",
            //                       parameterTypes.Select(p => (p as INamedTypeReference).Name.Value).CommaDelimitedList(),
            //                       parameterTypes.Select(p => String.Format("alteredp{0}", parameterCounter++)).CommaDelimitedList());
            var anonymousMethod = Anon.Action(parameterTypes.ToArray())
                .WithBody(c => c.AddLine(x =>
                                             {
                                                 var parameters = x.Params.ToList();

                                                 var originalMethodCall = Call.StaticMethod(staticAction)
                                                     .ThatReturnsVoid()
                                                     .WithArguments(parameters.Select(p => p as IExpression).ToArray())
                                                     .On(staticAction.ResolvedMethod.ContainingTypeDefinition);
                                                 return x.Do(originalMethodCall);
                                             }));
            Context.Log.WriteTrace("          };");
            Context.Block.Statements.Add(
                Declare.Variable("local_0", anonymousMethod.Type).As(anonymousMethod)
                );

            AddStatement.CallShouldInterceptOnInterceptor();
            AddStatement.SetOriginalCallOnInvocation();
            AddStatement.SetArgumentsOnInvocation();
            AddStatement.SetTargetOnInvocationToNull();
            AddStatement.SetOriginalCallInfoOnInvocation();
            AddStatement.CallInterceptOnInterceptor();

            Context.Block.Statements.Add(Return.Void());            
        }
    }
}