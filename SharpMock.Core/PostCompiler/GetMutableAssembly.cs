using Microsoft.Cci;
using Microsoft.Cci.ILToCodeModel;

namespace SharpMock.Core.PostCompiler
{
    public abstract class GetMutableAssembly : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            var testAssembly = context.Host.LoadUnitFrom(AssemblyPath(context)) as IAssembly;
            context.AssemblyToAlter = Decompiler.GetCodeModelFromMetadataModel(
                context.Host, testAssembly, null, DecompilerOptions.AnonymousDelegates | DecompilerOptions.Iterators | DecompilerOptions.Loops);
        }

        protected abstract string AssemblyPath(PostCompilerContext context);
    }
}