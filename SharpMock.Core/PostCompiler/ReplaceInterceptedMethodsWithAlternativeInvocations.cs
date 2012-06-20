using SharpMock.Core.PostCompiler.Replacement;

namespace SharpMock.Core.PostCompiler
{
    public class ReplaceInterceptedMethodsWithAlternativeInvocations : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var methodCallReplacer = new SpecifiedMethodCallReplacer(context.Host);
            methodCallReplacer.Visit(context.AssemblyToAlter);
        }
    }
}