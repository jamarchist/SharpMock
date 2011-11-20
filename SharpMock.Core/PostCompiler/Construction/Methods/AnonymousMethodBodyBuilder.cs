using System;
using System.Reflection;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    internal class AnonymousMethodBodyBuilder : IAnonymousMethodBodyBuilder
    {
        private readonly IMetadataHost host;
        private readonly IUnitReflector reflector;
        private readonly ITypeReference delegateType;
        private readonly Type returnType;
        private readonly ParameterInfo[] parameters;

        public AnonymousMethodBodyBuilder(IMetadataHost host, IUnitReflector reflector, ITypeReference delegateType, Type returnType, ParameterInfo[] parameters)
        {
            this.host = host;
            this.reflector = reflector;
            this.delegateType = delegateType;
            this.returnType = returnType;
            this.parameters = parameters;
        }

        public IExpression WithBody(VoidAction<ICodeBuilder> code)
        {
            var method = new AnonymousDelegate();
            method.Type = delegateType;
            method.CallingConvention = CallingConvention.HasThis;

            foreach (var parameter in parameters)
            {
                var parameterDefinition = new ParameterDefinition();
                parameterDefinition.Index = (ushort) parameter.Position;
                parameterDefinition.Type = reflector.Get(parameter.ParameterType);
                parameterDefinition.Name = host.NameTable.GetNameFor("altered" + parameter.Name);
                parameterDefinition.ContainingSignature = method;

                method.Parameters.Add(parameterDefinition);
            }

            //if (returnType != null && returnType != typeof(void)) 
                method.ReturnType = reflector.Get(returnType);

            var codeBuilder = new CodeBuilder(host, method.Parameters);
            code(codeBuilder);

            var body = new BlockStatement();
            foreach (var statement in codeBuilder.Statements) body.Statements.Add(statement);
            method.Body = body;

            return method;
        }
    }
}