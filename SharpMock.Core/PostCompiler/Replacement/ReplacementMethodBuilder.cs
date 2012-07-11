using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.PostCompiler.Construction.Declarations;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public abstract class ReplacementMethodBuilder : ReplacementMethodBuilderBase
    {
        protected ReplacementMethodBuilder(ReplacementMethodConstructionContext context) : base(context)
        {
        }

        protected override void BuildMethodTemplate()
        {
            Context.Log.WriteTrace(String.Empty);
            Context.Log.WriteTrace("BuildingMethod: {0}.", Context.OriginalCall.Name.Value);

            var ParamBindings = new Dictionary<string, IBoundExpression>();
            if (!Context.OriginalCall.ResolvedMethod.IsStatic && !Context.OriginalCall.ResolvedMethod.IsConstructor)
            {
                var targetBinding = new BoundExpression();
                var ps = new List<IParameterDefinition>(Context.FakeMethod.Parameters);
                targetBinding.Definition = ps[0];
                targetBinding.Type = ps[0].Type;

                ParamBindings.Add(ps[0].Name.Value, targetBinding);
            }

            Context.Log.WriteTrace("  Adding: var interceptor = new RegistryInterceptor();");
            Context.Block.Statements.Add(
                Declare.Variable<RegistryInterceptor>("interceptor").As(Create.New<RegistryInterceptor>())
            );

            Context.Log.WriteTrace("  Adding: var invocation = new Invocation();");
            Context.Block.Statements.Add(
                Declare.Variable<Invocation>("invocation").As(Create.New<Invocation>())
            );

            Context.Log.WriteTrace("  Adding: var interceptedType = typeof ({0});", 
                (Context.OriginalCall.ContainingType.ResolvedType as INamedEntity).Name.Value);
            Context.Block.Statements.Add(
                Declare.Variable<Type>("interceptedType").As(Operators.TypeOf(Context.OriginalCall.ContainingType.ResolvedType))
            );

            Context.Log.WriteTrace("  Adding: var parameterTypes = new Type[{0}];", Context.OriginalCall.ParameterCount);
            Context.Block.Statements.Add(
                Declare.Variable<Type[]>("parameterTypes").As(Create.NewArray<Type>(Context.OriginalCall.ParameterCount))
            );

            Context.Log.WriteTrace("  Adding: var arguments = new List<object>();");
            Context.Block.Statements.Add(
                Declare.Variable<List<object>>("arguments").As(Create.New<List<object>>())
            );

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

                Context.Log.WriteTrace("  Adding: parameterTypes[{0}] = typeof({1});", 
                    parameter.Index, (parameter.Type.ResolvedType as INamedEntity).Name.Value);
                Context.Block.Statements.Add(
                    Statements.Execute(assignment)
                );
            }

            Context.Log.WriteTrace("  Adding: var interceptedMethod = interceptedType.GetMethod('{0}', parameterTypes);", Context.OriginalCall.Name.Value);
            Context.Block.Statements.Add(
                DeclareMethodInfoVariable().As(CallGetMethodInfoMethod())
            );

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
            //foreach (var originalParameter in Context.FakeMethod.Parameters)
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

                    // ...
                    // arguments.Add(p0);
                    // ...
                    Context.Log.WriteTrace("  Adding: arguments.Add({0});", originalParameter.Name.Value);
                    var argumentAsObject = ChangeType.Box(argumentToAdd);
                    Context.Block.Statements.Add(
                        Statements.Execute(
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
                Statements.Execute(Call.PropertySetter<Delegate>("OriginalCall").WithArguments("local_0").On("invocation"))
            );

            Context.Log.WriteTrace("  Adding: invocation.Arguments = arguments;");
            Context.Block.Statements.Add(
                Statements.Execute(Call.PropertySetter<IList<object>>("Arguments").WithArguments("arguments").On("invocation"))
            );

            if (Context.OriginalCall.ResolvedMethod.IsStatic || Context.OriginalCall.ResolvedMethod.IsConstructor)
            {
                Context.Log.WriteTrace("  Adding: invocation.Target = null;");
                Context.Block.Statements.Add(
                    Statements.Execute(Call.PropertySetter<object>("Target").WithArguments(Constant.Of<object>(null)).On("invocation"))
                );             
            }
            else
            {
                Context.Log.WriteTrace("  Adding: invocation.Target = target;");
                Context.Block.Statements.Add(
                    Statements.Execute(Call.PropertySetter<object>("Target").WithArguments(ParamBindings["target"]).On("invocation"))
                );
            }

            Context.Log.WriteTrace("  Adding: invocation.OriginalCallInfo = interceptedMethod;");
            Context.Block.Statements.Add(
                Statements.Execute(Call.PropertySetter<MemberInfo>("OriginalCallInfo").WithArguments("interceptedMethod").On("invocation"))
            );

            Context.Log.WriteTrace("  Adding: interceptor.Intercept(invocation);");
            Context.Block.Statements.Add(
                Statements.Execute(Call.Method("Intercept", typeof(IInvocation)).ThatReturnsVoid().WithArguments("invocation").On("interceptor"))
            );

            // ...
            // return; | return interceptionResult;
            // ...
            Context.Log.WriteTrace("  Adding: <return statement>");
            var returnStatement = new ReturnStatement();
            // abstract
            var interceptionResultDeclaration = AddInterceptionResultHandling(returnStatement);
            AddInterceptionExtraResultHandling(interceptionResultDeclaration);
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

        protected virtual MethodCall CallGetMethodInfoMethod()
        {
            return Call.VirtualMethod("GetMethod", typeof(string), typeof(Type[]))
                    .ThatReturns<MethodInfo>()
                    .WithArguments(
                        Constant.Of<string>(Context.OriginalCall.Name.Value),
                        Locals["parameterTypes"])
                    .On("interceptedType");
        }

        protected virtual IDynamicDeclarationOptions DeclareMethodInfoVariable()
        {
            return Declare.Variable("interceptedMethod", Reflector.Get<MethodInfo>());
        }
    }
}