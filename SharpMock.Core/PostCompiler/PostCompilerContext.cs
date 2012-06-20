using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Diagnostics;
using SharpMock.PostCompiler.Core;

namespace SharpMock.Core.PostCompiler
{
    public class PostCompilerContext
    {
        public PostCompilerArgs Args { get; set; }
        public IMetadataHost Host { get; set; }
        public IAssembly SharpMockDelegateTypes { get; set; }
        public IUnit SharpMockCore { get; set; }
        public ILogger Log { get; set; }
        public Assembly AssemblyToAlter { get; set; }
    }
}