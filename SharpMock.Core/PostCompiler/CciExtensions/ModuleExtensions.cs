using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core.CciExtensions
{
    public static class ModuleExtensions
    {
        public static NamespaceTypeDefinition AddStaticClass(this Module module, string className, IMetadataHost host)
        {
            var @class = new NamespaceTypeDefinition();
            @class.ContainingUnitNamespace = module.UnitNamespaceRoot;
            @class.InternFactory = host.InternFactory;
            @class.IsClass = true;
            @class.IsStatic = true;
            @class.IsAbstract = true;
            @class.IsSealed = true;
            @class.Name = host.NameTable.GetNameFor(className);

            module.AllTypes.Add(@class);

            return @class;
        }

        public static NamespaceTypeDefinition AddStaticClass(this IUnitNamespace ns, Module module, string className, IMetadataHost host)
        {
            var @class = new NamespaceTypeDefinition();
            @class.ContainingUnitNamespace = ns;
            @class.InternFactory = host.InternFactory;
            @class.IsClass = true;
            @class.IsStatic = true;
            @class.IsAbstract = true;
            @class.IsSealed = true;
            @class.Name = host.NameTable.GetNameFor(className);

            @class.BaseClasses.Add(host.PlatformType.SystemObject);

            module.AllTypes.Add(@class);

            return @class;
        }
    }
}
