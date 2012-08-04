using System;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementInstanceFieldAssignmentBuilder : ReplacementMethodBuilderBase
    {
        private readonly IFieldReference field;

        public ReplacementInstanceFieldAssignmentBuilder(ReplacementMethodConstructionContext context, ReplaceableFieldInfo fieldInfo) : base(context)
        {
            var reflector = new UnitReflector(context.Host);
            var containingType = reflector.Get(fieldInfo.DeclaringType.FullName);
            field = reflector.From(containingType).GetField(fieldInfo.Name);
        }

        public override void BuildMethod()
        {
            AddStatement.DeclareInterceptedType(field.ContainingType.ResolvedType);

            Context.Log.WriteTrace("  Adding: var interceptedField = interceptedType.GetField('{0}');", field.Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable<FieldInfo>("interceptedField").As(
                    Call.VirtualMethod("GetField", typeof(string)).ThatReturns<FieldInfo>().WithArguments(
                        Constant.Of(field.Name.Value)).On("interceptedType"))
                );

            AddStatement.DeclareArgumentsList();
            AddStatement.AddArgumentToList(Params["assignedValue"]);

            var actionT = SharpMockTypes.Actions[1];
            var actionActualT = new GenericTypeInstanceReference();
            actionActualT.GenericType = actionT;
            actionActualT.GenericArguments.Add(field.Type);

            var assignment = new AnonymousDelegate();
            assignment.Type = actionActualT;
            assignment.ReturnType = Context.Host.PlatformType.SystemVoid;
            assignment.CallingConvention = CallingConvention.HasThis;

            var parameterDefinition = new ParameterDefinition();
            parameterDefinition.Index = 0;
            parameterDefinition.Type = field.Type;
            parameterDefinition.Name = Context.Host.NameTable.GetNameFor("alteredValue");
            parameterDefinition.ContainingSignature = assignment;

            assignment.Parameters.Add(parameterDefinition);

            var assignmentBody = new BlockStatement();
            var assignActualField = new ExpressionStatement();
            var actualField = new TargetExpression();
            actualField.Type = field.Type;
            actualField.Definition = field;
            actualField.Instance = Params["target"];
            var value = new BoundExpression();
            value.Type = field.Type;
            value.Definition = parameterDefinition;
            var assignValueToField = new Assignment();
            assignValueToField.Source = value;
            assignValueToField.Target = actualField;
            assignValueToField.Type = field.Type;
            assignActualField.Expression = assignValueToField;

            actualField.Type = field.Type;
            actualField.Definition = field;

            assignmentBody.Statements.Add(assignActualField);
            assignmentBody.Statements.Add(new ReturnStatement());
            assignment.Body = assignmentBody;

            Context.Block.Statements.Add(
                Declare.Variable("local_0", actionActualT).As(assignment)
                );

            AddStatement.DeclareRegistryInterceptor();
            AddStatement.DeclareInvocation();
            AddStatement.SetArgumentsOnInvocation();
            AddStatement.SetOriginalCallOnInvocation();
            AddStatement.SetTargetOnInvocationToTargetParameter();

            Context.Block.Statements.Add(
                Do(Call.PropertySetter<MemberInfo>("OriginalCallInfo").WithArguments("interceptedField").On("invocation"))
                );

            AddStatement.CallShouldInterceptOnInterceptor();
            AddStatement.CallInterceptOnInterceptor();

            Context.Block.Statements.Add(Return.Void());
        }
    }
}