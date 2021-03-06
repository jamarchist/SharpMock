using SharpMock.Core.PostCompiler.Replacement;

namespace SharpMock.Core.PostCompiler
{
    public class ReplaceStaticMethodCalls : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var replacer = new CodeUnderTestVisitor(context.Host, context.Log, context.Registry);
            replacer.TraverseChildren(context.AssemblyToAlter);
        }
    }
}