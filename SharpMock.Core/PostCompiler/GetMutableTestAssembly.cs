using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;

namespace SharpMock.Core.PostCompiler
{
    public class GetMutableTestAssembly : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var testAssembly = context.Host.LoadUnitFrom(context.Args.TestAssemblyPath) as IAssembly;
            context.AssemblyToAlter = Decompiler.GetCodeModelFromMetadataModel(
                context.Host, testAssembly, null, DecompilerOptions.AnonymousDelegates | DecompilerOptions.Iterators | DecompilerOptions.Loops);
        }
    }
}