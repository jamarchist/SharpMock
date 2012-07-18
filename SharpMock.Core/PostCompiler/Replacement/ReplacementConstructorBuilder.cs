using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Utility;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementConstructorBuilder : ReplacementMethodBuilderBase
    {
        private readonly IMethodReference constructor;

        public ReplacementConstructorBuilder(ReplacementMethodConstructionContext context, IMethodReference constructor) : base(context)
        {
            this.constructor = constructor;
        }

        public override void BuildMethod()
        {
            Context.Log.WriteTrace(String.Empty);
            Context.Log.WriteTrace("BuildingMethod: {0}.", constructor.Name.Value);

            AddStatement.DeclareRegistryInterceptor();
            AddStatement.DeclareInvocation();
            AddStatement.DeclareInterceptedType(constructor.ContainingType.ResolvedType);
            AddStatement.DeclareParameterTypesArray(constructor.ParameterCount);
            AddStatement.DeclareArgumentsList();

            foreach (var parameter in constructor.Parameters)
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

            Context.Log.WriteTrace("  Adding: var interceptedMethod = interceptedType.GetConstructor(parameterTypes);");
            Context.Block.Statements.Add(
                Declare.Variable<ConstructorInfo>("interceptedMethod").As(
                Call.VirtualMethod("GetConstructor", typeof(Type[]))
                    .ThatReturns<ConstructorInfo>()
                    .WithArguments(
                        Locals["parameterTypes"])
                    .On("interceptedType"))
            );


            var parameterTypes = constructor.Parameters.Select(p => p.Type);
            var parameterTypesWithReturnType = new List<ITypeReference>(parameterTypes);
            parameterTypesWithReturnType.Add(Context.ReturnType);

            var anonymousMethod = Anon.Func(parameterTypesWithReturnType.ToArray())
                .WithBody(c =>
                {
                    c.AddLine(x => x.Declare.Variable("originalConstructorResult", Context.ReturnType)
                      .As(x.Create.New(Context.ReturnType, parameterTypes.ToArray())));
                    c.AddLine(x => x.Return.Variable(x.Locals["originalConstructorResult"]));
                });
            Context.Block.Statements.Add(
                Declare.Variable("local_0", anonymousMethod.Type).As(anonymousMethod)
            );

            AddStatement.CallShouldInterceptOnInterceptor();
            AddStatement.SetOriginalCallOnInvocation();
            AddStatement.SetArgumentsOnInvocation();
            AddStatement.SetTargetOnInvocationToNull();
            AddStatement.SetOriginalCallInfoOnInvocation();
            AddStatement.CallInterceptOnInterceptor();

            Context.Log.WriteTrace("  Adding: var interceptionResult = ({0})invocation.Return;",
                (Context.ReturnType.ResolvedType as INamedEntity).Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable("interceptionResult", Context.ReturnType).As(
                    ChangeType.Convert(Call.PropertyGetter<object>("Return").On("invocation")).To(Context.ReturnType))
            );

            Context.Log.WriteTrace("  Adding: return interceptionResult;");
            Context.Block.Statements.Add(
                Return.Variable(Locals["interceptionResult"])
            );
        }
    }
}