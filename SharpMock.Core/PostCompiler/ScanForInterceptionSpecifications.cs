using SharpMock.Core.PostCompiler.Replacement;

namespace SharpMock.Core.PostCompiler
{
    public class ScanForInterceptionSpecifications : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var specifiedMethodCallRegistrar = new SpecifiedMethodCallRegistrar(context.Host);
            specifiedMethodCallRegistrar.Visit(context.AssemblyToAlter);
        }
    }
}