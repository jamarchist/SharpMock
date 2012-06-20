namespace SharpMock.Core.PostCompiler
{
    public class GetMutableTargetAssembly : GetMutableAssembly
    {
        protected override string AssemblyPath(PostCompilerContext context)
        {
            return context.Args.ReferencedAssemblyPath;
        }
    }
}