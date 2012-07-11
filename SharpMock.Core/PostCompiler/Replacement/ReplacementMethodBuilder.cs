using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

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
}