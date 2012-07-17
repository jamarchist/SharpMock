using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Utility;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementConstructorBuilder : ReplacementMethodBuilderBase
    {
        public ReplacementConstructorBuilder(ReplacementMethodConstructionContext context) : base(context)
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

            Context.Log.WriteTrace("  Adding: var interceptedMethod = interceptedType.GetConstructor(parameterTypes);");
            Context.Block.Statements.Add(
                Declare.Variable<ConstructorInfo>("interceptedMethod").As(
                Call.VirtualMethod("GetConstructor", typeof(Type[]))
                    .ThatReturns<ConstructorInfo>()
                    .WithArguments(
                        Locals["parameterTypes"])
                    .On("interceptedType"))
            );


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

            AddStatement.CallShouldInterceptOnInterceptor();
            AddStatement.SetOriginalCallOnInvocation();
            AddStatement.SetArgumentsOnInvocation();

            if (Context.OriginalCall.ResolvedMethod.IsStatic || Context.OriginalCall.ResolvedMethod.IsConstructor)
            {
                AddStatement.SetTargetOnInvocationToNull();
            }
            else
            {
                AddStatement.SetTargetOnInvocationToTargetParameter();
            }

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