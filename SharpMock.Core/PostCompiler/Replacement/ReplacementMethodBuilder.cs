using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.PostCompiler.Construction.Methods;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public abstract class ReplacementMethodBuilder : ReplacementMethodBuilderBase
    {
        protected ICommonStatementsAdder AddStatement { get; private set; }
        
        protected ReplacementMethodBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
            AddStatement = new CommonStatementsAdder(this, s => context.Block.Statements.Add(s), context.Log);
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

            AddInterceptedMethodDeclaration();

            // This may not actually be generic
            var openGenericFunction = GetOpenGenericFunction();

            var closedGenericFunction = new GenericTypeInstanceReference();
            closedGenericFunction.GenericType = openGenericFunction as INamedTypeReference; // cheating to compile
            foreach (var originalParameter in Context.OriginalCall.Parameters)
            {
                closedGenericFunction.GenericArguments.Add(originalParameter.Type);
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

            var fakeMethodParameters = new List<IParameterDefinition>(Context.FakeMethod.Parameters);
            for (var pIndex = 0; pIndex < fakeMethodParameters.Count; pIndex++)
            {
                var originalParameter = fakeMethodParameters[pIndex];

                var parameterDefinition = new ParameterDefinition();
                parameterDefinition.Index = originalParameter.Index;
                parameterDefinition.Type = originalParameter.Type;
                parameterDefinition.Name = Context.Host.NameTable.GetNameFor("altered" + originalParameter.Name.Value);
                parameterDefinition.ContainingSignature = anonymousMethod;

                anonymousMethod.Parameters.Add(parameterDefinition);

                if ((!Context.OriginalCall.ResolvedMethod.IsStatic && !Context.OriginalCall.ResolvedMethod.IsConstructor) && pIndex == 0)
                {
                    Context.Log.WriteTrace("    info: Skipping instance parameter - IsStatic: {0}, IsConstructor: {1}, pIndex: {2}",
                        Context.OriginalCall.ResolvedMethod.IsStatic, Context.OriginalCall.ResolvedMethod.IsConstructor, pIndex);
                }
                else
                {
                    var argumentToAdd = new BoundExpression();
                    argumentToAdd.Definition = originalParameter;
                    argumentToAdd.Type = originalParameter.Type;

                    Context.Log.WriteTrace("  Adding: arguments.Add({0});", originalParameter.Name.Value);
                    var argumentAsObject = ChangeType.Box(argumentToAdd);
                    Context.Block.Statements.Add(
                        Do(
                            Call.VirtualMethod("Add", typeof(object)).ThatReturnsVoid().WithArguments(argumentAsObject).On("arguments")
                        )
                    );                    
                }
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
            if (Context.OriginalCall.ResolvedMethod.IsStatic || Context.OriginalCall.ResolvedMethod.IsConstructor)
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

                if (Context.OriginalCall.ContainingType.ResolvedType.IsInterface || Context.OriginalCall.ContainingType.ResolvedType.IsAbstract)
                {
                    originalMethodCall = Call.VirtualMethod(Context.OriginalCall)
                                            .ThatReturns(Context.OriginalCall.Type)
                                            .WithArguments(originalCallArguments.ToArray())
                                            .On(target);
                }
                else
                {
                    originalMethodCall = Call.Method(Context.OriginalCall)
                                            .ThatReturns(Context.OriginalCall.Type)
                                            .WithArguments(originalCallArguments.ToArray())
                                            .On(target);                    
                }
            }


            var anonymousMethodBody = new BlockStatement();
            anonymousMethod.Body = anonymousMethodBody;

            // Code inside the anonymous method (a call to the original method)
            var anonymousMethodReturnStatement = new ReturnStatement();
            AddOriginalMethodCallStatement(anonymousMethodBody, anonymousMethodReturnStatement, originalMethodCall);
            anonymousMethodBody.Statements.Add(anonymousMethodReturnStatement);

            // ...
            // Func<#TYPE#> local_0 = (alteredP1, alteredP2) => target.SomeMethod(alteredP1, alteredP2);
            // ...
            Context.Log.WriteTrace("  Adding: <local_0 declaration>");
            Context.Block.Statements.Add(
                Declare.Variable("local_0", func).As(anonymousMethod)
            );

            Context.Log.WriteTrace("  Adding: interceptor.ShouldIntercept(interceptedMethod);");
            Context.Block.Statements.Add(
                Declare.Variable<bool>("shouldIntercept").As(
                    Call.VirtualMethod("ShouldIntercept", typeof(IInvocation)).ThatReturns<bool>().WithArguments("invocation").On("interceptor"))
            );

            Context.Log.WriteTrace("  Adding: invocation.OriginalCall = local_0;");
            Context.Block.Statements.Add(
                Do(Call.PropertySetter<Delegate>("OriginalCall").WithArguments("local_0").On("invocation"))
            );

            Context.Log.WriteTrace("  Adding: invocation.Arguments = arguments;");
            Context.Block.Statements.Add(
                Do(Call.PropertySetter<IList<object>>("Arguments").WithArguments("arguments").On("invocation"))
            );

            if (Context.OriginalCall.ResolvedMethod.IsStatic || Context.OriginalCall.ResolvedMethod.IsConstructor)
            {
                Context.Log.WriteTrace("  Adding: invocation.Target = null;");
                Context.Block.Statements.Add(
                    Do(Call.PropertySetter<object>("Target").WithArguments(Constant.Of<object>(null)).On("invocation"))
                );             
            }
            else
            {
                Context.Log.WriteTrace("  Adding: invocation.Target = target;");
                Context.Block.Statements.Add(
                    Do(Call.PropertySetter<object>("Target").WithArguments(Params["target"]).On("invocation"))
                );
            }

            Context.Log.WriteTrace("  Adding: invocation.OriginalCallInfo = interceptedMethod;");
            Context.Block.Statements.Add(
                Do(Call.PropertySetter<MemberInfo>("OriginalCallInfo").WithArguments("interceptedMethod").On("invocation"))
            );

            Context.Log.WriteTrace("  Adding: interceptor.Intercept(invocation);");
            Context.Block.Statements.Add(
                Do(Call.Method("Intercept", typeof(IInvocation)).ThatReturnsVoid().WithArguments("invocation").On("interceptor"))
            );

            AddReturnStatement();
        }

        protected abstract void AddReturnTypeSpecificGenericArguments(GenericTypeInstanceReference closedGenericFunction);

        protected abstract ITypeReference GetOpenGenericFunction();

        protected abstract void AddOriginalMethodCallStatement(BlockStatement anonymousMethodBody, ReturnStatement anonymousMethodReturnStatement, MethodCall originalMethodCall);

        protected virtual void AddInterceptedMethodDeclaration()
        {
            Context.Log.WriteTrace("  Adding: var interceptedMethod = interceptedType.GetMethod('{0}', parameterTypes);"
                , Context.OriginalCall.Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable<MethodInfo>("interceptedMethod").As(
                Call.VirtualMethod("GetMethod", typeof(string), typeof(Type[]))
                    .ThatReturns<MethodInfo>()
                    .WithArguments(
                        Constant.Of(Context.OriginalCall.Name.Value), 
                        Locals["parameterTypes"])
                    .On("interceptedType"))
            );
        }

        protected abstract void AddReturnStatement();
    }

    public interface ICommonStatementsAdder
    {
        void DeclareRegistryInterceptor();
        void DeclareInvocation();
        void DeclareInterceptedType(ITypeDefinition type);
        void DeclareParameterTypesArray(int length);
        void DeclareArgumentsList();
        void AssignParameterTypeValue(int index, ITypeDefinition type);

    }

    public class CommonStatementsAdder : ICommonStatementsAdder
    {
        private readonly ILogger log;
        private readonly IMethodBodyBuilder builder;
        private readonly VoidAction<IStatement> add; 

        public CommonStatementsAdder(IMethodBodyBuilder builder, VoidAction<IStatement> add, ILogger log)
        {
            this.builder = builder;
            this.add = add;
            this.log = log;
        }

        public void DeclareRegistryInterceptor()
        {
            log.WriteTrace("  Adding: var interceptor = new RegistryInterceptor();");
            add(
                builder.Declare.Variable<RegistryInterceptor>("interceptor").As(builder.Create.New<RegistryInterceptor>())
            );
        }

        public void DeclareInvocation()
        {
            log.WriteTrace("  Adding: var invocation = new Invocation();");
            add(
                builder.Declare.Variable<Invocation>("invocation").As(builder.Create.New<Invocation>())
            );
        }

        public void DeclareInterceptedType(ITypeDefinition type)
        {
            log.WriteTrace("  Adding: var interceptedType = typeof ({0});",
                (type as INamedEntity).Name.Value);
            add(
                builder.Declare.Variable<Type>("interceptedType").As(builder.Operators.TypeOf(type))
            );
        }

        public void DeclareParameterTypesArray(int length)
        {
            log.WriteTrace("  Adding: var parameterTypes = new Type[{0}];", length);
            add(
                builder.Declare.Variable<Type[]>("parameterTypes").As(builder.Create.NewArray<Type>(length))
            );
        }

        public void DeclareArgumentsList()
        {
            log.WriteTrace("  Adding: var arguments = new List<object>();");
            add(
                builder.Declare.Variable<List<object>>("arguments").As(builder.Create.New<List<object>>())
            );
        }

        public void AssignParameterTypeValue(int index, ITypeDefinition type)
        {
            log.WriteTrace("  Adding: parameterTypes[{0}] = typeof({1});", index, (type as INamedEntity).Name.Value);
            add(
                builder.Locals.Array<Type>("parameterTypes")[index].Assign(builder.Operators.TypeOf(type))
            );
        }
    }
}