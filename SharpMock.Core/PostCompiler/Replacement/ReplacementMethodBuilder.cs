using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.PostCompiler.Construction;
using SharpMock.Core.PostCompiler.Construction.ControlFlow;
using SharpMock.Core.PostCompiler.Construction.Conversions;
using SharpMock.Core.PostCompiler.Construction.Declarations;
using SharpMock.Core.PostCompiler.Construction.Definitions;
using SharpMock.Core.PostCompiler.Construction.Expressions;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public abstract class ReplacementMethodBuilder : IReplacementMethodBuilder
    {
        protected SharpMockTypes SharpMockTypes { get; private set; }
        protected ReplacementMethodConstructionContext Context { get; private set; }
        protected IUnitReflector Reflector { get; private set; }
        protected IDefinitionBuilder Define { get; private set; }
        protected IDeclarationBuilder Declare { get; private set; }
        protected ILocalVariableBindings Locals { get; private set; }
        protected IInstanceCreator Create { get; private set; }
        protected IMethodCallBuilder Call { get; private set; }
        protected IConverter ChangeType { get; private set; }
        protected IStatementBuilder Statements { get; private set; }
        protected ITypeOperatorBuilder Operators { get; private set; }
        protected ICompileTimeConstantBuilder Constant { get; private set; }
        protected IIfStatementBuilder If { get; private set; }

        protected ReplacementMethodBuilder(ReplacementMethodConstructionContext context)
        {
            Context = context;
            SharpMockTypes = new SharpMockTypes(context.Host);
        }

        private void CreateDslContext()
        {
            Reflector = new UnitReflector(Context.Host);
            Locals = new LocalVariableBindings(Reflector);
            Define = new DefinitionBuilder(Reflector, Locals, Context.Host.NameTable);
            Create = new InstanceCreator(Reflector);
            Declare = new DeclarationBuilder(Define);
            Call = new MethodCallBuilder(Context.Host, Reflector, Locals);
            ChangeType = new Converter(Reflector);
            Statements = new StatementBuilder();
            Operators = new TypeOperatorBuilder(Reflector);
            Constant = new CompileTimeConstantBuilder(Reflector);
            If = new IfStatementBuilder();
        }

        private void BuildMethodTemplate()
        {
            var ParamBindings = new Dictionary<string, IBoundExpression>();
            if (!Context.OriginalCall.ResolvedMethod.IsStatic)
            {
                var targetBinding = new BoundExpression();
                var ps = new List<IParameterDefinition>(Context.FakeMethod.Parameters);
                targetBinding.Definition = ps[0];
                targetBinding.Type = ps[0].Type;

                ParamBindings.Add(ps[0].Name.Value, targetBinding);
            }

            //  ...
            //  var interceptedType = typeof (#SOMETYPE#);
            //  ...
            var interceptedTypeDeclaration =
                Declare.Variable<Type>("interceptedType").As(Operators.TypeOf(Context.OriginalCall.ContainingType.ResolvedType));

            //  ...
            //  var parameterTypes = new Type[#SOMESIZE#];
            //  ...
            var parameterTypesDeclaration =
                Declare.Variable<Type[]>("parameterTypes").As(Create.NewArray<Type>(Context.OriginalCall.ParameterCount));

            //  ...
            //  parameterTypes[#SOMEINDEX#] = typeof (#SOMETYPE#);
            //  ...
            var arrayElementAssignments = new List<IStatement>();
            foreach (var parameter in Context.OriginalCall.Parameters)
            {
                var indexer = new ArrayIndexer();
                indexer.IndexedObject = Locals["parameterTypes"];
                indexer.Indices.Add(new CompileTimeConstant { Type = Reflector.Get<int>(), Value = parameter.Index });
                indexer.Type = Reflector.Get<Type>();

                var target = new TargetExpression();
                target.Definition = indexer;
                target.Instance = Locals["parameterTypes"];
                target.Type = Reflector.Get<Type[]>();

                var assignment = new Assignment();
                assignment.Type = Reflector.Get<Type>();
                assignment.Source = Operators.TypeOf(parameter.Type);
                assignment.Target = target;

                arrayElementAssignments.Add(
                    Statements.Execute(
                        assignment
                        )
                    );
            }

            //  ...
            //  var interceptedMethod = interceptedType.GetMethod(#SOMEMETHODNAME#, parameterTypes);
            //  ...
            var interceptedMethodDeclaration = Declare.Variable<MethodInfo>("interceptedMethod").As(
                Call.VirtualMethod("GetMethod", typeof (string), typeof (Type[]))
                    .ThatReturns<MethodInfo>()
                    .WithArguments(
                        Constant.Of<string>(Context.OriginalCall.Name.Value),
                        Locals["parameterTypes"])
                    .On("interceptedType")
                );

            //  ... 
            //  var arguments = new List<object>();
            //  ...
            var genericListOfObjectsDeclaration =
                Declare.Variable<List<object>>("arguments").As(Create.New<List<object>>());

            // This may not actually be generic
            var openGenericFunction = GetOpenGenericFunction();

            var closedGenericFunction = new GenericTypeInstanceReference();
            closedGenericFunction.GenericType = openGenericFunction as INamedTypeReference; // cheating to compile
            foreach (var originalParameter in Context.OriginalCall.Parameters)
            {
                closedGenericFunction.GenericArguments.Add(originalParameter.Type);

                //if (!Context.OriginalCall.ResolvedMethod.IsStatic)
                //{
                //    closedGenericFunction.GenericArguments.RemoveAt(0);
                //}
            }
            // abstract
            AddReturnTypeSpecificGenericArguments(closedGenericFunction);

            closedGenericFunction.InternFactory = Context.Host.InternFactory;
            closedGenericFunction.TypeCode = PrimitiveTypeCode.NotPrimitive;
            closedGenericFunction.PlatformType = Context.Host.PlatformType;

            ITypeReference func = openGenericFunction;
            if (openGenericFunction.ResolvedType.IsGeneric)
            {
                func = closedGenericFunction;
            }

            var anonymousMethod = new AnonymousDelegate();
            anonymousMethod.Type = func; //closedGenericFunction;
            anonymousMethod.ReturnType = Context.FakeMethod.Type;
            anonymousMethod.CallingConvention = CallingConvention.HasThis;

            // Arguments
            // get the add method
            var addMethodCallStatements = new List<ExpressionStatement>();

            foreach (var originalParameter in Context.FakeMethod.Parameters)
            {
                var parameterDefinition = new ParameterDefinition();
                parameterDefinition.Index = originalParameter.Index;
                parameterDefinition.Type = originalParameter.Type;
                parameterDefinition.Name = Context.Host.NameTable.GetNameFor("altered" + originalParameter.Name.Value);
                parameterDefinition.ContainingSignature = anonymousMethod;

                anonymousMethod.Parameters.Add(parameterDefinition);

                var argumentToAdd = new BoundExpression();
                argumentToAdd.Definition = originalParameter;
                argumentToAdd.Type = originalParameter.Type;

                // ...
                // arguments.Add(p0);
                // ...
                var argumentAsObject = ChangeType.Box(argumentToAdd);
                var addArgumentCallExpression = Statements.Execute(
                     Call.VirtualMethod("Add", typeof(object))
                         .ThatReturnsVoid()
                         .WithArguments(argumentAsObject)
                         .On("arguments")
                    );

                addMethodCallStatements.Add(addArgumentCallExpression);
            }

            if (!Context.OriginalCall.ResolvedMethod.IsStatic)
            {
                addMethodCallStatements.RemoveAt(0);
            }

            var originalCallArguments = new List<IExpression>();
            foreach (var parameter in anonymousMethod.Parameters)
            {
                var boundArgument = new BoundExpression();
                boundArgument.Definition = parameter;
                boundArgument.Type = parameter.Type;

                originalCallArguments.Add(boundArgument);
            }

            // ...
            // 
            // ...
            MethodCall originalMethodCall = null;
            if (Context.OriginalCall.ResolvedMethod.IsStatic)
            {
                originalMethodCall = Call.StaticMethod(Context.OriginalCall)
                                            .ThatReturns(Context.OriginalCall.Type)
                                            .WithArguments(originalCallArguments.ToArray()) 
                                            .On(Context.OriginalCall.ResolvedMethod.ContainingTypeDefinition);               
            }
            else
            {
                var target = originalCallArguments[0];
                originalCallArguments.RemoveAt(0);

                originalMethodCall = Call.Method(Context.OriginalCall)
                                        .ThatReturns(Context.OriginalCall.Type)
                                        .WithArguments(originalCallArguments.ToArray())
                                        .On(target);
            }


            var anonymousMethodBody = new BlockStatement();
            anonymousMethod.Body = anonymousMethodBody;

            // Code inside the anonymous method (a call to the original method)
            var anonymousMethodReturnStatement = new ReturnStatement();
            AddOriginalMethodCallStatement(anonymousMethodBody, anonymousMethodReturnStatement, originalMethodCall);
            anonymousMethodBody.Statements.Add(anonymousMethodReturnStatement);

            var delegateDeclaration = Declare.Variable("local_0", func).As(anonymousMethod);

            //  ...
            //  var interceptor = new RegistryInterceptor();
            //  ...
            var registryInterceptorDeclaration =
                Declare.Variable<RegistryInterceptor>("interceptor").As(Create.New<RegistryInterceptor>());

            //  ...
            //  interceptor.ShouldIntercept(interceptedMethod);
            //  ...
            var shouldInterceptCall = Declare.Variable<bool>("shouldIntercept").As(
                Call.VirtualMethod("ShouldIntercept", typeof(MethodInfo), typeof(IList<object>)).ThatReturns<bool>().WithArguments("interceptedMethod", "arguments").On("interceptor"));

            //  ...
            //  var invocation = new Invocation();
            //  ...
            var invocationObjectDeclaration = Declare.Variable<Invocation>("invocation").As(Create.New<Invocation>());

            // ...
            // invocation.OriginalCall = local_0;
            // ...
            var setDelegateStatement = Statements.Execute(
                Call.PropertySetter<Delegate>("OriginalCall").WithArguments("local_0").On("invocation"));

            // ...
            // invocation.Target = null;
            // ...
            ExpressionStatement setTargetStatement = null;
            if (Context.OriginalCall.ResolvedMethod.IsStatic)
            {
                setTargetStatement = Statements.Execute(
                    Call.PropertySetter<object>("Target").WithArguments(Constant.Of<object>(null)).On("invocation"));                
            }
            else
            {
                setTargetStatement = Statements.Execute(
                    Call.PropertySetter<object>("Target").WithArguments(ParamBindings["target"]).On("invocation"));
            }

            // ...
            // invocation.Arguments = arguments;
            // ...
            var setArgumentsStatement = Statements.Execute(
                    Call.PropertySetter<IList<object>>("Arguments").WithArguments("arguments").On("invocation"));

            // ...
            // interceptor.Intercept(invocation);
            // ...
            var interceptMethodCallStatement = Statements.Execute(
                    Call.Method("Intercept", typeof(IInvocation)).ThatReturnsVoid().WithArguments("invocation").On("interceptor"));

            // ...
            // return; | return interceptionResult;
            // ...
            var returnStatement = new ReturnStatement();

            // abstract
            var interceptionResultDeclaration = AddInterceptionResultHandling(returnStatement);

            Context.Block.Statements.Add(registryInterceptorDeclaration);
            //WriteOut("registry interceptor declared");
            Context.Block.Statements.Add(invocationObjectDeclaration);
            //WriteOut("invocation object declared");
            Context.Block.Statements.Add(interceptedTypeDeclaration);
            //WriteOut("intercepted type declared");
            Context.Block.Statements.Add(parameterTypesDeclaration);
            //WriteOut("parameter types declared");
            foreach (var arrayElementAssigment in arrayElementAssignments)
            {
                Context.Block.Statements.Add(arrayElementAssigment);
                //WriteOut("array element assigned");
            }
            Context.Block.Statements.Add(interceptedMethodDeclaration);
            //WriteOut("intercepted method declared");
            Context.Block.Statements.Add(delegateDeclaration);
            //WriteOut("delegate declared");
            Context.Block.Statements.Add(genericListOfObjectsDeclaration);
            //WriteOut("generic list of objects declared");
            foreach (var addStatement in addMethodCallStatements)
            {
                Context.Block.Statements.Add(addStatement);
                //WriteOut("method call statement added");
            }

            Context.Block.Statements.Add(setDelegateStatement);
            //WriteOut("delegate set");
            Context.Block.Statements.Add(setArgumentsStatement);
            //WriteOut("argument set");
            Context.Block.Statements.Add(setTargetStatement);

            Context.Block.Statements.Add(shouldInterceptCall);
            //WriteOut("shouldintercept called");
            Context.Block.Statements.Add(interceptMethodCallStatement);
            //WriteOut("intercept called");

            AddInterceptionExtraResultHandling(interceptionResultDeclaration);
            //WriteOut("result declared");
            
            Context.Block.Statements.Add(returnStatement);
        }

        private void WriteOut(string message)
        {
            //Context.Block.Statements.Add(
            //    Statements.Execute(
            //        Call.StaticMethod("WriteLine", typeof(string)).ThatReturnsVoid().WithArguments(Constant.Of<string>(message)).On(typeof(Console))
            //    )
            //);
        }

        protected abstract void AddInterceptionExtraResultHandling(LocalDeclarationStatement interceptionResultDeclaration);

        protected abstract LocalDeclarationStatement AddInterceptionResultHandling(ReturnStatement returnStatement);

        protected abstract void AddReturnTypeSpecificGenericArguments(GenericTypeInstanceReference closedGenericFunction);

        protected abstract ITypeReference GetOpenGenericFunction();

        protected abstract void AddOriginalMethodCallStatement(BlockStatement anonymousMethodBody, ReturnStatement anonymousMethodReturnStatement, MethodCall originalMethodCall);

        public void BuildMethod()
        {
            CreateDslContext();
            BuildMethodTemplate();
        }
    }
}