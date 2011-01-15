using System;
using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.DelegateTypes;

namespace SharpMock.PostCompiler.Core
{
    public abstract class ReplacementMethodBuilder : IReplacementMethodBuilder
    {
        protected SharpMockTypes sharpMockTypes { get; private set; }
        protected ReplacementMethodConstructionContext Context { get; private set; }
        protected IUnitReflector Reflector { get; private set; }
        protected IDefinitionBuilder Define { get; private set; }
        protected IDeclarationBuilder Declare { get; private set; }
        protected ILocalVariableBindings Locals { get; private set; }
        protected IInstanceCreator Create { get; private set; }
        protected IPropertySetter Properties { get; private set; }

        protected ReplacementMethodBuilder(ReplacementMethodConstructionContext context)
        {
            Context = context;
            sharpMockTypes = new SharpMockTypes(context.Host);
        }

        private void BuildMethodTemplate()
        {
            var coreAssembly = Context.Host.LoadUnit(Context.Host.CoreAssemblySymbolicIdentity);
            Reflector = new UnitReflector(Context.Host);
            Locals = new LocalVariableBindings();
            Define = new DefinitionBuilder(Reflector, Locals, Context.Host.NameTable);
            Create = new InstanceCreator(Reflector);
            Declare = new DeclarationBuilder(Define);
            Properties = new PropertySetter(Locals, Reflector, Context.Host);

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

            var delegateDeclaration = new LocalDeclarationStatement();
            var delegateDefinition = new LocalDefinition();

            delegateDefinition.Type = closedGenericFunction;
            delegateDefinition.Name = Context.Host.NameTable.GetNameFor("local_0");

            var anonymousMethod = new AnonymousDelegate();
            anonymousMethod.Type = closedGenericFunction;
            anonymousMethod.ReturnType = Context.FakeMethod.Type;
            anonymousMethod.CallingConvention = CallingConvention.HasThis;

            // Arguments
            // get the add method
            var genericListAdd = Reflector.From<List<object>>().GetMethod("Add", typeof (object));
            var addMethodCallStatements = new List<IStatement>();

            Locals.AddBinding("local_0", delegateDefinition, closedGenericFunction);

            Code(delegateDeclaration);
            //Context.Block.Statements.Add(genericListOfObjectsDeclaration);
            Code(Declare.Variable<List<object>>("arguments").As(Create.New<List<object>>()));

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

                IExpression finalArgumentToAdd = argumentToAdd;

                if (argumentToAdd.Type.IsValueType)
                {
                    var conversion = new Conversion();
                    conversion.TypeAfterConversion = Context.Host.PlatformType.SystemObject;
                    conversion.ValueToConvert = argumentToAdd;

                    finalArgumentToAdd = conversion;
                }

                var addArgument = new MethodCall();
                addArgument.MethodToCall = genericListAdd;
                addArgument.ThisArgument = Locals["arguments"];
                addArgument.Type = Context.Host.PlatformType.SystemVoid;
                addArgument.Arguments.Add(finalArgumentToAdd);
                addArgument.IsVirtualCall = true;

                var addArgumentCallExpression = new ExpressionStatement();
                addArgumentCallExpression.Expression = addArgument;

                addMethodCallStatements.Add(addArgumentCallExpression);
            }

            Code(addMethodCallStatements);
            Code(Declare.Variable<Invocation>("invocation").As(Create.New<Invocation>()));

            var originalMethodCall = new MethodCall();
            var toCall = TypeHelper.GetMethod(Context.OriginalCall.ResolvedMethod.ContainingTypeDefinition, Context.OriginalCall);

            originalMethodCall.MethodToCall = toCall;
            originalMethodCall.Type = toCall.Type;
            originalMethodCall.IsStaticCall = true;

            foreach (var parameter in anonymousMethod.Parameters)
            {
                var boundArgument = new BoundExpression();
                boundArgument.Definition = parameter;
                boundArgument.Type = parameter.Type;

                originalMethodCall.Arguments.Add(boundArgument);
            }

            //var codeBuilder = new BlockBuilder(host);
            var anonymousMethodBody = new BlockStatement();
            anonymousMethod.Body = anonymousMethodBody;

            // Code inside the anonymous method (a call to the original method)
            var anonymousMethodReturnStatement = new ReturnStatement();
            

            AddOriginalMethodCallStatement(anonymousMethodBody, anonymousMethodReturnStatement, originalMethodCall);
            
            anonymousMethodBody.Statements.Add(anonymousMethodReturnStatement);

            delegateDeclaration.LocalVariable = delegateDefinition;
            delegateDeclaration.InitialValue = anonymousMethod;

            var @null2 = new CompileTimeConstant();
            @null2.Type = Context.Host.PlatformType.SystemObject;

            Code( Properties.On<Invocation>("invocation").Set<Delegate>("OriginalCall").To("local_0") );
            Code( Properties.On<Invocation>("invocation").Set<IList<object>>("Arguments").To("arguments") );
            Code( Properties.On<Invocation>("invocation").Set<object>("Target").To(@null2) );

            Code(Declare.Variable<RegistryInterceptor>("interceptor").As(Create.New<RegistryInterceptor>()));

            //var registryInterceptorDeclaration =
            //    Declare.Variable<RegistryInterceptor>("interceptor").As(Create.New<RegistryInterceptor>());
            var interceptor = Locals["interceptor"];

            var interceptMethod = Reflector.From<RegistryInterceptor>().GetMethod("Intercept", typeof(IInvocation));
            
            var interceptMethodCall = new MethodCall();
            interceptMethodCall.MethodToCall = interceptMethod;
            interceptMethodCall.ThisArgument = interceptor;
            interceptMethodCall.Type = Context.Host.PlatformType.SystemVoid;
            interceptMethodCall.Arguments.Add(Locals["invocation"]);

            var interceptMethodCallStatement = new ExpressionStatement();
            interceptMethodCallStatement.Expression = interceptMethodCall;

            var returnStatement = new ReturnStatement();
            //var interceptionResultDeclaration = new LocalDeclarationStatement();
            //var interceptionResultDeclaration = Declare.Variable("interceptionResult")
            // abstract
            var interceptionResultDeclaration = AddInterceptionResultHandling(returnStatement);
           
            Code( interceptMethodCallStatement );

            AddInterceptionExtraResultHandling(interceptionResultDeclaration);
             
            Code( returnStatement );
        }

        private void Code(IStatement statement)
        {
            Context.Block.Statements.Add(statement);
        }

        private void Code(IEnumerable<IStatement> statements)
        {
            foreach (var statement in statements)
            {
                Context.Block.Statements.Add(statement);
            }
        }

        protected abstract void AddInterceptionExtraResultHandling(LocalDeclarationStatement interceptionResultDeclaration);

        protected abstract LocalDeclarationStatement AddInterceptionResultHandling(ReturnStatement returnStatement);

        protected abstract void AddReturnTypeSpecificGenericArguments(GenericTypeInstanceReference closedGenericFunction);

        protected abstract ITypeReference GetOpenGenericFunction();

        protected abstract void AddOriginalMethodCallStatement(BlockStatement anonymousMethodBody, ReturnStatement anonymousMethodReturnStatement, MethodCall originalMethodCall);

        public void BuildMethod()
        {
            BuildMethodTemplate();
        }
    }
}