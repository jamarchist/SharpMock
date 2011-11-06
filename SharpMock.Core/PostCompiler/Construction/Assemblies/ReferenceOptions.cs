namespace SharpMock.Core.PostCompiler.Construction.Assemblies
{
    internal class ReferenceOptions : IReferenceOptions
    {
        private readonly AssemblyConfiguration config;

        public ReferenceOptions(AssemblyConfiguration config)
        {
            this.config = config;
        }

        public void Assembly(string assemblyLocation)
        {
            config.ReferencePaths.Add(assemblyLocation);    
        }
    }
}