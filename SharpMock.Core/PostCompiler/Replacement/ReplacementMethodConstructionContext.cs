using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementMethodConstructionContext
    {
        private readonly IMetadataHost host;
        private readonly IMethodReference originalCall;
        private readonly BlockStatement block;
        private readonly IFieldReference originalField;
        private readonly bool isAssignment;
        private readonly ILogger log;
        private readonly IEnumerable<IParameterDefinition> fakeMethodParameters;
        private readonly ITypeReference returnType;

        public IMetadataHost Host { get { return host; } }
        public BlockStatement Block { get { return block; } }
        public ILogger Log { get { return log; } }
        public IEnumerable<IParameterDefinition> FakeMethodParameters { get { return fakeMethodParameters; } }
        public ITypeReference ReturnType { get { return returnType; } }

        public ReplacementMethodConstructionContext(IMetadataHost host, IMethodReference originalCall, IMethodDefinition fakeMethod, BlockStatement block, ILogger log)
        {
            this.host = host;
            this.block = block;
            this.log = log;
            this.originalCall = originalCall;

            fakeMethodParameters = fakeMethod.Parameters;
            returnType = fakeMethod.Type;
        }

        public ReplacementMethodConstructionContext(IMetadataHost host, IFieldReference originalField, IMethodDefinition fakeMethod, BlockStatement block, bool isAssignment, ILogger log)
        {
            this.host = host;
            this.block = block;
            this.originalField = originalField;
            this.isAssignment = isAssignment;
            this.log = log;

            fakeMethodParameters = fakeMethod.Parameters;
            returnType = fakeMethod.Type;
        }

        public IReplacementMethodBuilder GetMethodBuilder()
        {
            if (originalCall == null && !isAssignment)
            {
                return new ReplacementFieldAccessorBuilder(this, originalField);
            }

            if (originalCall == null && isAssignment)
            {
                return new ReplacementStaticFieldAssignmentBuilder(this, originalField);
            }

            if (originalCall.ResolvedMethod.IsConstructor)
            {
                return new ReplacementConstructorBuilder(this, originalCall);
            }

            var hasOutOrRefParameters = false;
            foreach (var parameter in originalCall.ResolvedMethod.Parameters)
            {
                if (parameter.IsOut || parameter.IsByReference)
                {
                    hasOutOrRefParameters = true;
                    break;
                }
            }

            if (hasOutOrRefParameters)
            {
                
            }

            if (!originalCall.Type.ResolvedType.Equals(host.PlatformType.SystemVoid.ResolvedType))
            {
                if (originalCall.IsStatic)
                {
                    return new ReplacementStaticFunctionBuilder(this, originalCall);    
                }

                if (originalCall.ResolvedMethod.IsAbstract || originalCall.ResolvedMethod.ContainingType.ResolvedType.IsInterface)
                {
                    return new ReplacementAbstractInstanceFunctionBuilder(this, originalCall);
                }

                return new ReplacementInstanceFunctionBuilder(this, originalCall);
            }
            else
            {
                if (originalCall.IsStatic)
                {
                    return new ReplacementStaticActionBuilder(this, originalCall);
                }
                
                if (originalCall.ResolvedMethod.IsAbstract || originalCall.ResolvedMethod.ContainingType.ResolvedType.IsInterface)
                {
                    return new ReplacementAbstractInstanceActionBuilder(this, originalCall);
                }

                return new ReplacementInstanceActionBuilder(this, originalCall);
            }
        }
    }
}