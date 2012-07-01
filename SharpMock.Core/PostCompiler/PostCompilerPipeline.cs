using System.Collections.Generic;

namespace SharpMock.Core.PostCompiler
{
    public class PostCompilerPipeline
    {
        private readonly IList<IPostCompilerPipelineStep> steps = new List<IPostCompilerPipelineStep>();

        public void Execute(PostCompilerContext context)
        {
            context.Log.WriteInfo("Executing PostCompilerPipeline.");
            foreach (var step in steps)
            {
                context.Log.WriteTrace("Executing step '{0}'.", step.GetType().Name);
                step.Execute(context);
            }
        }

        public void AddStep(IPostCompilerPipelineStep step)
        {
            steps.Add(step);
        }
    }
}