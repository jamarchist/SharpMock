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

        public IMetadataHost Host { get { return host; } }
        public IMethodReference OriginalCall { get { return originalCall; } }
        public IMethodDefinition FakeMethod { get { return fakeMethod; } }
        public BlockStatement Block { get { return block; } }

        public ReplacementMethodConstructionContext(IMetadataHost host, IMethodReference originalCall, IMethodDefinition fakeMethod, BlockStatement block)
        {
            this.host = host;
            this.block = block;
            this.fakeMethod = fakeMethod;
            this.originalCall = originalCall;
        }

        public IReplacementMethodBuilder GetMethodBuilder()
        {
            if (originalCall.ResolvedMethod.IsConstructor)
            {
                return new ReplacementConstructorBuilder(this);
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