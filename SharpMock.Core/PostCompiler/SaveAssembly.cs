using System.IO;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler
{
    public abstract class SaveAssembly : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            using (var assemblyStream = File.OpenWrite(AssemblyName(context)))
            {
                PeWriter.WritePeToStream(context.AssemblyToAlter, context.Host, assemblyStream);
                assemblyStream.Close();
            }
        }

        protected abstract string AssemblyName(PostCompilerContext context);
    }
}