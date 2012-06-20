namespace SharpMock.Core.PostCompiler
{
    public class AddReferenceToSharpMockTypesAssembly : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            context.AssemblyToAlter.AssemblyReferences.Add(context.SharpMockDelegateTypes);
        }
    }

    
}