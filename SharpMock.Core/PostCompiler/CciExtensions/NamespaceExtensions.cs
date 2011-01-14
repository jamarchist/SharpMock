using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CciExtensions
{
    public static class NamespaceExtensions
    {
        public static NestedUnitNamespace AddNestedNamespace(this IUnitNamespace parent, string namespaceName, IMetadataHost host)
        {
            var child = new NestedUnitNamespace();
            child.ContainingUnitNamespace = parent;
            child.Name = host.NameTable.GetNameFor(namespaceName);
            //parent.Members.Add(child);
            var p = parent as UnitNamespace;
            p.Members.Add(child);

            return child;
        }
    }
}
