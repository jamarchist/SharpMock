namespace SharpMock.Core.PostCompiler
{
    public interface IPostCompilerPipelineStep
    {
        void Execute(PostCompilerContext context);
    }
}