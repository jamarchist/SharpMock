using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.PostCompiler.Construction;
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

        protected ReplacementMethodBuilder(ReplacementMethodConstructionContext context)
        {
            Context = context;
            SharpMockTypes = new SharpMockTypes(context.Host);
        }

        private void CreateDslContext()
        {
            Reflector = new UnitReflector(Context.Host);
            Locals = new LocalVariableBindings();
            Define = new DefinitionBuilder(Reflector, Locals, Context.Host.NameTable);
            Create = new InstanceCreator(Reflector);
            Declare = new DeclarationBuilder(Define);
            Call = new MethodCallBuilder(Context.Host, Reflector, Locals);
            ChangeType = new Converter(Reflector);
            Statements = new StatementBuilder();
            Operators = new TypeOperatorBuilder(Reflector);
            Constant = new CompileTimeConstantBuilder(Reflector);
        }

        private void BuildMethodTemplate()
        {
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

            var openGenericFunction = GetOpenGenericFunction();

            var closedGenericFunction = new GenericTypeInstanceReference();
            closedGenericFunction.GenericType = openGenericFunction;
            foreach (var originalParameter in Context.FakeMethod.Parameters)
            {
                closedGenericFunction.GenericArguments.Add(originalParameter.Type);
            }
            // abstract
            AddReturnTypeSpecificGenericArguments(closedGenericFunction);

            closedGenericFunction.InternFactory = Context.Host.InternFactory;
            closedGenericFunction.TypeCode = PrimitiveTypeCode.NotPrimitive;
            closedGenericFunction.PlatformType = Context.Host.PlatformType;

            var anonymousMethod = new AnonymousDelegate();
            anonymousMethod.Type = closedGenericFunction;
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
            var originalMethodCall = Call.StaticMethod(Context.OriginalCall)
                                        .ThatReturns(Context.OriginalCall.Type)
                                        .WithArguments(originalCallArguments.ToArray()) 
                                        .On(Context.OriginalCall.ResolvedMethod.ContainingTypeDefinition);

            var anonymousMethodBody = new BlockStatement();
            anonymousMethod.Body = anonymousMethodBody;

            // Code inside the anonymous method (a call to the original method)
            var anonymousMethodReturnStatement = new ReturnStatement();
            AddOriginalMethodCallStatement(anonymousMethodBody, anonymousMethodReturnStatement, originalMethodCall);
            anonymousMethodBody.Statements.Add(anonymousMethodReturnStatement);

            var delegateDeclaration = Declare.Variable("local_0", closedGenericFunction).As(anonymousMethod);

            //  ...
            //  var interceptor = new RegistryInterceptor();
            //  ...
            var registryInterceptorDeclaration =
                Declare.Variable<RegistryInterceptor>("interceptor").As(Create.New<RegistryInterceptor>());

            //  ...
            //  if (interceptor.ShouldIntercept(interceptedMethod))
            //  {
            //  ...
            var ifStatement = new ConditionalStatement();
            ifStatement.Condition =
                Call.Method("ShouldIntercept").ThatReturns<bool>().WithArguments("interceptedMethod").On("interceptor");

            //  ...
            //  } // end if
            //  ...

            //  ...
            //  var invocation = new Invocation();
            //  ...
            var invocationObjectDeclaration = Declare.Variable<Invocation>("invocation").As(Create.New<Invocation>());

            // ...
            // invocation.OriginalCall = local_0;
            // ...
            var setDelegateStatement = Statements.Execute(
                    Call.PropertySetter<Delegate>("OriginalCall").WithArguments("local_0").On("invocation")
                );

            var @null2 = new CompileTimeConstant();
            @null2.Type = Context.Host.PlatformType.SystemObject;

            // ...
            // invocation.Target = null;
            // ...
            var setTargetStatement = Statements.Execute(
                    Call.PropertySetter<object>("Target").WithArguments(@null2).On("invocation")
                );

            // ...
            // invocation.Arguments = arguments;
            // ...
            var setArgumentsStatement = Statements.Execute(
                    Call.PropertySetter<IList<object>>("Arguments").WithArguments("arguments").On("invocation")
                );

            // ...
            // interceptor.Intercept(invocation);
            // ...
            var interceptMethodCallStatement = Statements.Execute(
                    Call.Method("Intercept", typeof(IInvocation)).ThatReturnsVoid().WithArguments("invocation").On("interceptor")
                );

            // ...
            // return; | return interceptionResult;
            // ...
            var returnStatement = new ReturnStatement();

            // abstract
            var interceptionResultDeclaration = AddInterceptionResultHandling(returnStatement);

            Context.Block.Statements.Add(invocationObjectDeclaration);
            Context.Block.Statements.Add(interceptedTypeDeclaration);
            Context.Block.Statements.Add(parameterTypesDeclaration);
            foreach (var arrayElementAssigment in arrayElementAssignments)
            {
                Context.Block.Statements.Add(arrayElementAssigment);
            }
            Context.Block.Statements.Add(interceptedMethodDeclaration);
            Context.Block.Statements.Add(delegateDeclaration);
            Context.Block.Statements.Add(genericListOfObjectsDeclaration);
            foreach (var addStatement in addMethodCallStatements)
            {
                Context.Block.Statements.Add(addStatement);
            }

            Context.Block.Statements.Add(setDelegateStatement);
            Context.Block.Statements.Add(setArgumentsStatement);
            Context.Block.Statements.Add(setTargetStatement);

            Context.Block.Statements.Add(registryInterceptorDeclaration);
            Context.Block.Statements.Add(interceptMethodCallStatement);

            AddInterceptionExtraResultHandling(interceptionResultDeclaration);
            
            Context.Block.Statements.Add(returnStatement);
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