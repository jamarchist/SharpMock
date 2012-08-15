using System.Collections.Generic;
using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.ControlFlow;
using SharpMock.Core.PostCompiler.Construction.Conversions;
using SharpMock.Core.PostCompiler.Construction.Declarations;
using SharpMock.Core.PostCompiler.Construction.Definitions;
using SharpMock.Core.PostCompiler.Construction.Expressions;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public class MethodBodyBuilder : IMethodBodyBuilder
    {
        private readonly IUnitReflector reflector;
        private readonly ILocalVariableBindings locals;
        private readonly IDefinitionBuilder define;
        private readonly IInstanceCreator create;
        private readonly IDeclarationBuilder declare;
        private readonly IMethodCallBuilder call;
        private readonly IConverter changeType;
        private readonly ITypeOperatorBuilder operators;
        private readonly ICompileTimeConstantBuilder constant;
        private readonly IIfStatementBuilder @if;
        private readonly ICodeReturnStatementBuilder @return;
        private readonly IAnonymousMethodTypeOptions anonymousMethod;
        private readonly IStatementBuilder statement;
        private readonly IParameterBindings @params;

        public MethodBodyBuilder(IMetadataHost host, IEnumerable<IParameterDefinition> parameters)
        {
            reflector = new UnitReflector(host);
            locals = new LocalVariableBindings(reflector);
            define = new DefinitionBuilder(reflector, locals, host.NameTable);
            create = new InstanceCreator(reflector, locals);
            declare = new DeclarationBuilder(define);
            call = new MethodCallBuilder(host, reflector, locals);
            changeType = new Converter(reflector);
            operators = new TypeOperatorBuilder(reflector);
            constant = new CompileTimeConstantBuilder(reflector);
            @if = new IfStatementBuilder();
            @return = new CodeReturnStatementBuilder();
            anonymousMethod = new AnonymousMethodTypeOptions(host, reflector);
            statement = new StatementBuilder();
            @params = new ParameterBindings();

            foreach (var parameter in parameters)
            {
                @params.AddBinding(parameter);
            }
        }

        public IUnitReflector Reflector
        {
            get { return reflector; }
        }

        public ILocalVariableBindings Locals
        {
            get { return locals; }
        }

        public IParameterBindings Params
        {
            get { return @params; }
        }

        public IDefinitionBuilder Define
        {
            get { return define; }
        }

        public IInstanceCreator Create
        {
            get { return create; }
        }

        public IDeclarationBuilder Declare
        {
            get { return declare; }
        }

        public IMethodCallBuilder Call
        {
            get { return call; }
        }

        public IConverter ChangeType
        {
            get { return changeType; }
        }

        public ITypeOperatorBuilder Operators
        {
            get { return operators; }
        }

        public ICompileTimeConstantBuilder Constant
        {
            get { return constant; }
        }

        public IIfStatementBuilder If
        {
            get { return @if; }
        }

        public ICodeReturnStatementBuilder Return
        {
            get { return @return; }
        }

        public IAnonymousMethodTypeOptions Anon
        {
            get { return anonymousMethod; }
        }

        public IStatement Do(IExpression expression)
        {
            return statement.Execute(expression);
        }
    }
}