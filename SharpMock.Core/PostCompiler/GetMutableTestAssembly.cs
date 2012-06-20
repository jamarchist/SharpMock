namespace SharpMock.Core.PostCompiler
{
    public class GetMutableTestAssembly : GetMutableAssembly
    {
        protected override string AssemblyPath(PostCompilerContext context)
        {
            return context.Args.TestAssemblyPath;
        }
    }
}