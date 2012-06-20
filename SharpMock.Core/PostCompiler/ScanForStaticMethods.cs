using SharpMock.Core.PostCompiler.Replacement;

namespace SharpMock.Core.PostCompiler
{
    public class ScanForStaticMethods : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var staticMethodCallRegistrar = 
                new StaticMethodCallRegistrar(context.Host, context.AssemblyToAlter.Location);
            staticMethodCallRegistrar.Visit(context.AssemblyToAlter);
        }
    }
}