using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementFieldAccessorBuilder : ReplacementMethodBuilderBase
    {
        public ReplacementFieldAccessorBuilder(ReplacementMethodConstructionContext context) : base(context)
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
                    Call.VirtualMethod("GetField", typeof (string)).ThatReturns<FieldInfo>().WithArguments(
                        Constant.Of(Context.OriginalField.Name.Value)).On("interceptedType"));

            //  ... 
            //  var arguments = new List<object>();
            //  ...
            var genericListOfObjectsDeclaration =
                Declare.Variable<List<object>>("arguments").As(Create.New<List<object>>());

            var funcT = SharpMockTypes.Functions[0];
            var funcActualT = new GenericTypeInstanceReference();
            funcActualT.GenericType = funcT;
            funcActualT.GenericArguments.Add(Context.OriginalField.Type);

            var accessor = new AnonymousDelegate();
            accessor.Type = funcActualT;
            accessor.ReturnType = Context.OriginalField.Type;
            accessor.CallingConvention = CallingConvention.HasThis;

            var accessorBody = new BlockStatement();
            var returnActualField = new ReturnStatement();
            var actualField = new BoundExpression();
            actualField.Type = Context.OriginalField.Type;
            actualField.Definition = Context.OriginalField;
            returnActualField.Expression = actualField;
            accessorBody.Statements.Add(returnActualField);
            accessor.Body = accessorBody;

            var accessorDeclaration = Declare.Variable("local_0", funcActualT).As(accessor);

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
            // var interceptionResult = (#SOMETYPE#)invocation.Return;
            // ...
            var interceptionResultDeclaration = Declare.Variable("interceptionResult", Context.OriginalField.Type).As(
                ChangeType.Convert(Call.PropertyGetter<object>("Return").On("invocation")).To(Context.OriginalField.Type));

            // ...
            // return interceptionResult;
            // ...
            var returnStatement = new ReturnStatement();
            returnStatement.Expression = Locals["interceptionResult"];

            Context.Block.Statements.Add(registryInterceptorDeclaration);
            Context.Block.Statements.Add(invocationObjectDeclaration);
            Context.Block.Statements.Add(interceptedTypeDeclaration);
            Context.Block.Statements.Add(interceptedMethodDeclaration);
            Context.Block.Statements.Add(accessorDeclaration);
            Context.Block.Statements.Add(genericListOfObjectsDeclaration);
            Context.Block.Statements.Add(setDelegateStatement);
            Context.Block.Statements.Add(setArgumentsStatement);
            Context.Block.Statements.Add(setTargetStatement);
            Context.Block.Statements.Add(setOriginalCallInfoStatement);
            Context.Block.Statements.Add(shouldInterceptCall);
            Context.Block.Statements.Add(interceptMethodCallStatement);
            Context.Block.Statements.Add(interceptionResultDeclaration);
            Context.Block.Statements.Add(returnStatement);
        }
    }
}