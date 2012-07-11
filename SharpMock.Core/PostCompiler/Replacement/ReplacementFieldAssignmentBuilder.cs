using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementFieldAssignmentBuilder : ReplacementMethodBuilderBase
    {
        public ReplacementFieldAssignmentBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
        }

        protected override void BuildMethodTemplate()
        {
            //  ...
            //  var interceptedType = typeof (#SOMETYPE#);
            //  ...
            var interceptedTypeDeclaration =
                Declare.Variable<Type>("interceptedType").As(Operators.TypeOf(Context.OriginalField.ContainingType.ResolvedType));

            //  ...
            //  var interceptedField = interceptedType.GetField(#SOMEFIELDNAME#);
            //  ...
            var interceptedMethodDeclaration =
                Declare.Variable<FieldInfo>("interceptedField").As(
                    Call.VirtualMethod("GetField", typeof(string)).ThatReturns<FieldInfo>().WithArguments(
                        Constant.Of(Context.OriginalField.Name.Value)).On("interceptedType"));

            //  ... 
            //  var arguments = new List<object>();
            //  ...
            var genericListOfObjectsDeclaration =
                Declare.Variable<List<object>>("arguments").As(Create.New<List<object>>());

            // ...
            // arguments.Add(#p0#);
            // ...
            var parameters = new List<IParameterDefinition>(Context.FakeMethod.Parameters);
            var argumentToAdd = new BoundExpression();
            argumentToAdd.Definition = parameters[0];
            argumentToAdd.Type = Context.OriginalField.Type;

            var argumentAsObject = ChangeType.Box(argumentToAdd);
            var addArgumentCallExpression = Do(
                 Call.VirtualMethod("Add", typeof(object))
                     .ThatReturnsVoid()
                     .WithArguments(argumentAsObject)
                     .On("arguments")
                );

            var actionT = SharpMockTypes.Actions[1];
            var actionActualT = new GenericTypeInstanceReference();
            actionActualT.GenericType = actionT;
            actionActualT.GenericArguments.Add(Context.OriginalField.Type);

            var assignment = new AnonymousDelegate();
            assignment.Type = actionActualT;
            assignment.ReturnType = Context.Host.PlatformType.SystemVoid;
            assignment.CallingConvention = CallingConvention.HasThis;

            var parameterDefinition = new ParameterDefinition();
            parameterDefinition.Index = 0;
            parameterDefinition.Type = Context.OriginalField.Type;
            parameterDefinition.Name = Context.Host.NameTable.GetNameFor("alteredValue");
            parameterDefinition.ContainingSignature = assignment;

            assignment.Parameters.Add(parameterDefinition);

            var assignmentBody = new BlockStatement();
            var assignActualField = new ExpressionStatement();
            var actualField = new TargetExpression();
            actualField.Type = Context.OriginalField.Type;
            actualField.Definition = Context.OriginalField;
            var value = new BoundExpression();
            value.Type = Context.OriginalField.Type;
            value.Definition = parameterDefinition;
            var assignValueToField = new Assignment();
            assignValueToField.Source = value;
            assignValueToField.Target = actualField;
            assignValueToField.Type = Context.OriginalField.Type;
            assignActualField.Expression = assignValueToField;

            actualField.Type = Context.OriginalField.Type;
            actualField.Definition = Context.OriginalField;
            
            assignmentBody.Statements.Add(assignActualField);
            assignmentBody.Statements.Add(new ReturnStatement());
            assignment.Body = assignmentBody;

            var assignmentDeclaration = Declare.Variable("local_0", actionActualT).As(assignment);

            //  ...
            //  var interceptor = new RegistryInterceptor();
            //  ...
            var registryInterceptorDeclaration =
                Declare.Variable<RegistryInterceptor>("interceptor").As(Create.New<RegistryInterceptor>());

            //  ...
            //  var invocation = new Invocation();
            //  ...
            var invocationObjectDeclaration = Declare.Variable<Invocation>("invocation").As(Create.New<Invocation>());

            //  ...
            //  interceptor.ShouldIntercept(interceptedMethod);
            //  ...
            var shouldInterceptCall = Declare.Variable<bool>("shouldIntercept").As(
                Call.VirtualMethod("ShouldIntercept", typeof(IInvocation)).ThatReturns<bool>().WithArguments("invocation").On("interceptor"));

            // ...
            // invocation.OriginalCall = local_0;
            // ...
            var setDelegateStatement = Do(
                Call.PropertySetter<Delegate>("OriginalCall").WithArguments("local_0").On("invocation"));

            // ...
            // invocation.Target = null;
            // ...
            var setTargetStatement = Do(
                Call.PropertySetter<object>("Target").WithArguments(Constant.Of<object>(null)).On("invocation"));

            // ...
            // invocation.OriginalCallInfo = interceptedField;
            // ...
            var setOriginalCallInfoStatement = Do(
                Call.PropertySetter<MemberInfo>("OriginalCallInfo").WithArguments("interceptedField").On("invocation"));

            // ...
            // invocation.Arguments = arguments;
            // ...
            var setArgumentsStatement = Do(
                Call.PropertySetter<IList<object>>("Arguments").WithArguments("arguments").On("invocation"));

            // ...
            // interceptor.Intercept(invocation);
            // ...
            var interceptMethodCallStatement = Do(
                Call.Method("Intercept", typeof(IInvocation)).ThatReturnsVoid().WithArguments("invocation").On("interceptor"));

            // ...
            // return;
            // ...
            var returnStatement = new ReturnStatement();

            Context.Block.Statements.Add(registryInterceptorDeclaration);
            Context.Block.Statements.Add(invocationObjectDeclaration);
            Context.Block.Statements.Add(interceptedTypeDeclaration);
            Context.Block.Statements.Add(interceptedMethodDeclaration);
            Context.Block.Statements.Add(genericListOfObjectsDeclaration);
            Context.Block.Statements.Add(addArgumentCallExpression);
            Context.Block.Statements.Add(assignmentDeclaration);
            Context.Block.Statements.Add(setDelegateStatement);
            Context.Block.Statements.Add(setArgumentsStatement);
            Context.Block.Statements.Add(setTargetStatement);
            Context.Block.Statements.Add(setOriginalCallInfoStatement);
            Context.Block.Statements.Add(shouldInterceptCall);
            Context.Block.Statements.Add(interceptMethodCallStatement);
            Context.Block.Statements.Add(returnStatement);
        }
    }
}