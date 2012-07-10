using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementMethodConstructionContext
    {
        private readonly IMetadataHost host;
        private readonly IMethodReference originalCall;
        private readonly IMethodDefinition fakeMethod;
        private readonly BlockStatement block;
        private readonly IFieldReference originalField;
        private readonly bool isAssignment;

        public IMetadataHost Host { get { return host; } }
        public IMethodReference OriginalCall { get { return originalCall; } }
        public IMethodDefinition FakeMethod { get { return fakeMethod; } }
        public BlockStatement Block { get { return block; } }
        public IFieldReference OriginalField { get { return originalField; } }

        public ReplacementMethodConstructionContext(IMetadataHost host, IMethodReference originalCall, IMethodDefinition fakeMethod, BlockStatement block)
        {
            this.host = host;
            this.block = block;
            this.fakeMethod = fakeMethod;
            this.originalCall = originalCall;
        }

        public ReplacementMethodConstructionContext(IMetadataHost host, IFieldReference originalField, IMethodDefinition fakeMethod, BlockStatement block, bool isAssignment)
        {
            this.host = host;
            this.block = block;
            this.fakeMethod = fakeMethod;
            this.originalField = originalField;
            this.isAssignment = isAssignment;
        }

        public IReplacementMethodBuilder GetMethodBuilder()
        {
            if (originalCall == null && !isAssignment)
            {
                return new ReplacementFieldAccessorBuilder(this);
            }

            if (originalCall == null && isAssignment)
            {
                return new ReplacementFieldAssignmentBuilder(this);
            }

            if (originalCall.ResolvedMethod.IsConstructor)
            {
                return new ReplacementConstructorBuilder(this);
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
                return new ReplacementFunctionBuilder(this);
            }
            else
            {
                return new ReplacementActionBuilder(this);
            }
        }
    }
}