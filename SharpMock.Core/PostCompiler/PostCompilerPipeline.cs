using System.Collections.Generic;

namespace SharpMock.Core.PostCompiler
{
    public class PostCompilerPipeline
    {
        private readonly IList<IPostCompilerPipelineStep> steps = new List<IPostCompilerPipelineStep>();

        public void Execute(PostCompilerContext context)
        {
            foreach (var step in steps)
            {
                step.Execute(context);
            }
        }

        public void AddStep(IPostCompilerPipelineStep step)
        {
            steps.Add(step);
        }
    }
}