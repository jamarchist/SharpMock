namespace SharpMock.Core.PostCompiler
{
    public class SaveTestAssembly : SaveAssembly
    {
        protected override string AssemblyName(PostCompilerContext context)
        {
            return context.Args.TestAssemblyPath;
        }
    }
}