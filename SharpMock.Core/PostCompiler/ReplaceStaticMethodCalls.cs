using SharpMock.Core.PostCompiler.Replacement;

namespace SharpMock.Core.PostCompiler
{
    public class ReplaceStaticMethodCalls : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var replacer = new StaticMethodCallReplacer(context.Host, context.Log);
            replacer.Visit(context.AssemblyToAlter);
        }
    }
}