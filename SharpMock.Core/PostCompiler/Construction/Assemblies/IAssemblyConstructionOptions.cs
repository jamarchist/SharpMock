namespace SharpMock.Core.PostCompiler.Construction.Assemblies
{
    public interface IAssemblyConstructionOptions
    {
        void Name(string assemblyName);
        ITypeOptions Type { get; }
    }
}