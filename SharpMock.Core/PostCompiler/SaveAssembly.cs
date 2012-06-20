using System.IO;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler
{
    public class SaveAssembly : IPostCompilerPipelineStep
    {
        public void Execute(PostCompilerContext context)
        {
            using(var assemblyStream = File.OpenWrite(context.Args.TestAssemblyPath))
            {
                PeWriter.WritePeToStream(context.AssemblyToAlter, context.Host, assemblyStream);
                assemblyStream.Close();                
            }
        }
    }
}