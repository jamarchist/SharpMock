using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Cci;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception;
using SharpMock.Core.Interception.Interceptors;
using SharpMock.Core.PostCompiler.Construction.Methods;

namespace SharpMock.Core.PostCompiler.Replacement
{
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
            var typeName = "<unknown>";
            var namedType = type as INamedEntity;
            if (namedType == null)
            {
                var vector = type as IArrayTypeReference;
                if (vector != null)
                {
                    namedType = vector.ElementType as INamedEntity;
                }
            }

            if (namedType != null) typeName = namedType.Name.Value;

            log.WriteTrace("  Adding: parameterTypes[{0}] = typeof({1});", index, typeName);
            add(
                builder.Locals.Array<Type>("parameterTypes")[index].Assign(builder.Operators.TypeOf(type))
                );
        }

        public void AddArgumentToList(IBoundExpression argument)
        {
            log.WriteTrace("  Adding: arguments.Add({0});", (argument.Definition as IParameterDefinition).Name.Value);
            add(
                builder.Do(
                    builder.Call.VirtualMethod("Add", typeof (object)).ThatReturnsVoid().WithArguments(builder.ChangeType.Box(argument)).On(
                        "arguments")
                    )
                );
        }

        public void CallShouldInterceptOnInterceptor()
        {
            log.WriteTrace("  Adding: interceptor.ShouldIntercept(interceptedMethod);");
            add(
                builder.Declare.Variable<bool>("shouldIntercept").As(
                    builder.Call.VirtualMethod("ShouldIntercept", typeof(IInvocation)).ThatReturns<bool>().WithArguments("invocation").On("interceptor"))
                );
        }

        public void SetOriginalCallOnInvocation()
        {
            log.WriteTrace("  Adding: invocation.OriginalCall = local_0;");
            add(
                builder.Do(builder.Call.PropertySetter<Delegate>("OriginalCall").WithArguments("local_0").On("invocation"))
                );
        }

        public void SetArgumentsOnInvocation()
        {
            log.WriteTrace("  Adding: invocation.Arguments = arguments;");
            add(
                builder.Do(builder.Call.PropertySetter<IList<object>>("Arguments").WithArguments("arguments").On("invocation"))
                );
        }

        public void SetTargetOnInvocationToNull()
        {
            log.WriteTrace("  Adding: invocation.Target = null;");
            SetTargetOnInvocationTo(builder.Constant.Of<object>(null));
        }

        public void SetTargetOnInvocationToTargetParameter()
        {
            log.WriteTrace("  Adding: invocation.Target = target;");
            SetTargetOnInvocationTo(builder.Params["target"]);
        }

        private void SetTargetOnInvocationTo(IExpression target)
        {
            add(
                builder.Do(builder.Call.PropertySetter<object>("Target").WithArguments(target).On("invocation"))
                );            
        }

        public void SetOriginalCallInfoOnInvocation()
        {
            log.WriteTrace("  Adding: invocation.OriginalCallInfo = interceptedMethod;");
            add(
                builder.Do(builder.Call.PropertySetter<MemberInfo>("OriginalCallInfo").WithArguments("interceptedMethod").On("invocation"))
                );
        }

        public void CallInterceptOnInterceptor()
        {
            log.WriteTrace("  Adding: interceptor.Intercept(invocation);");
            add(
                builder.Do(builder.Call.Method("Intercept", typeof(IInvocation)).ThatReturnsVoid().WithArguments("invocation").On("interceptor"))
                );
        }
    }
}