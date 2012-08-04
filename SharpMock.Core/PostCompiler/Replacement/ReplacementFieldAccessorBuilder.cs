using System;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementFieldAccessorBuilder : ReplacementMethodBuilderBase
    {
        private readonly IFieldReference field;

        public ReplacementFieldAccessorBuilder(ReplacementMethodConstructionContext context, ReplaceableFieldInfo fieldInfo) : base(context)
        {
            var reflector = new UnitReflector(context.Host);
            field = reflector.From(fieldInfo.DeclaringType.FullName).GetField(fieldInfo.Name);
        }

        public override void BuildMethod()
        {
            AddStatement.DeclareInterceptedType(field.ContainingType.ResolvedType);

            Context.Log.WriteTrace("  Adding: var interceptedField = interceptedType.GetField('{0}');", field.Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable<FieldInfo>("interceptedField").As(
                    Call.VirtualMethod("GetField", typeof (string)).ThatReturns<FieldInfo>().WithArguments(
                        Constant.Of(field.Name.Value)).On("interceptedType"))
            );

            AddStatement.DeclareArgumentsList();

            var funcT = SharpMockTypes.Functions[0];
            var funcActualT = new GenericTypeInstanceReference();
            funcActualT.GenericType = funcT;
            funcActualT.GenericArguments.Add(field.Type);

            var accessor = new AnonymousDelegate();
            accessor.Type = funcActualT;
            accessor.ReturnType = field.Type;
            accessor.CallingConvention = CallingConvention.HasThis;

            var accessorBody = new BlockStatement();
            var returnActualField = new ReturnStatement();
            var actualField = new BoundExpression();
            actualField.Type = field.Type;
            actualField.Definition = field;
            returnActualField.Expression = actualField;
            accessorBody.Statements.Add(returnActualField);
            accessor.Body = accessorBody;

            Context.Block.Statements.Add(
                Declare.Variable("local_0", funcActualT).As(accessor)
            );

            AddStatement.DeclareRegistryInterceptor();
            AddStatement.DeclareInvocation();
            AddStatement.SetArgumentsOnInvocation();
            AddStatement.SetOriginalCallOnInvocation();
            AddStatement.SetTargetOnInvocationToNull();
            
            Context.Block.Statements.Add(
                Do(Call.PropertySetter<MemberInfo>("OriginalCallInfo").WithArguments("interceptedField").On("invocation"))
            );

            AddStatement.CallShouldInterceptOnInterceptor();            
            AddStatement.CallInterceptOnInterceptor();

            Context.Block.Statements.Add(
                Declare.Variable("interceptionResult", field.Type).As(
                    ChangeType.Convert(Call.PropertyGetter<object>("Return").On("invocation")).To(field.Type))
            );

            Context.Block.Statements.Add(Return.Variable(Locals["interceptionResult"]));
        }
    }
}