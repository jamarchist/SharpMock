using System;
using System.Collections.Generic;
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

        private readonly AnonymousDelegate method = new AnonymousDelegate();
        private readonly ITypeReference returnTypeReference;

        public AnonymousMethodBodyBuilder(IMetadataHost host, IUnitReflector reflector, ITypeReference delegateType, ITypeReference returnType, KeyValuePair<string, ITypeReference>[] parameterTypes)
        {
            this.host = host;
            this.reflector = reflector;
            this.delegateType = delegateType;
            this.returnTypeReference = returnType;

            for (var pIndex = 0; pIndex < parameterTypes.Length; pIndex++)
            {
                var parameterType = parameterTypes[pIndex];

                var parameterDefinition = new ParameterDefinition();
                parameterDefinition.Index = (ushort) pIndex;
                parameterDefinition.Type = parameterType.Value;
                parameterDefinition.Name = host.NameTable.GetNameFor("altered" + parameterType.Key);
                parameterDefinition.ContainingSignature = method;

                method.Parameters.Add(parameterDefinition);                
            }
        }

        public AnonymousMethodBodyBuilder(IMetadataHost host, IUnitReflector reflector, ITypeReference delegateType, Type returnType, ParameterInfo[] parameters)
        {
            this.host = host;
            this.reflector = reflector;
            this.delegateType = delegateType;

            this.returnTypeReference = reflector.Get(returnType);

            foreach (var parameter in parameters)
            {
                var parameterDefinition = new ParameterDefinition();
                parameterDefinition.Index = (ushort)parameter.Position;
                parameterDefinition.Type = reflector.Get(parameter.ParameterType);
                parameterDefinition.Name = host.NameTable.GetNameFor("altered" + parameter.Name);
                parameterDefinition.ContainingSignature = method;

                method.Parameters.Add(parameterDefinition);
            }
        }

        public IExpression WithBody(VoidAction<ICodeBuilder> code)
        {
            method.Type = delegateType;
            method.CallingConvention = CallingConvention.HasThis;
            method.ReturnType = returnTypeReference;

            var codeBuilder = new CodeBuilder(host, method.Parameters);
            code(codeBuilder);

            var body = new BlockStatement();
            foreach (var statement in codeBuilder.Statements) body.Statements.Add(statement);
            method.Body = body;

            return method;
        }
    }
}