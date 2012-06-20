namespace SharpMock.Core.PostCompiler
{
    public class SaveTargetAssembly : SaveAssembly
    {
        protected override string AssemblyName(PostCompilerContext context)
        {
            return context.Args.ReferencedAssemblyPath;
        }
    }
}