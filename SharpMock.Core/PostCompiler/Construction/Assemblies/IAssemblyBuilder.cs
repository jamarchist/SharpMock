using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Assemblies
{
    public interface IAssemblyBuilder
    {
        IModule CreateNewDll(VoidAction<IAssemblyConstructionOptions> with);
    }
}
