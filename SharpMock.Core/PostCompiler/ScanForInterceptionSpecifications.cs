using SharpMock.Core.PostCompiler.Replacement;

namespace SharpMock.Core.PostCompiler
{
    public class ScanForInterceptionSpecifications : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var specifiedMethodCallRegistrar = new SpecifiedMethodCallRegistrar(context.Host, context.Log, context.Registry);
            specifiedMethodCallRegistrar.TraverseChildren(context.AssemblyToAlter);
        }
    }
}