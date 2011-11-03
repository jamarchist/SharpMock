namespace SharpMock.Core.PostCompiler.Construction.Assemblies
{
    public interface IAssemblyBuilder
    {
        void CreateNewDll(VoidAction<IAssemblyConstructionOptions> with);
    }
}
