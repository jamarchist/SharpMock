using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core
{
    public class SharpMockTypes
    {
        private readonly IDictionary<int, INamedTypeReference> functions = new Dictionary<int, INamedTypeReference>();
        private readonly IDictionary<int, INamedTypeReference> actions = new Dictionary<int, INamedTypeReference>();

        public IDictionary<int, INamedTypeReference> Functions { get { return functions; } }
        public IDictionary<int, INamedTypeReference> Actions { get { return actions; } }

        public IUnit Unit { get; private set; }
        
        public SharpMockTypes(IMetadataHost host)
        {
            var sharpMockTypes =
                host.LoadUnitFrom(
                    System.Reflection.Assembly.GetExecutingAssembly().Location);
            var sharpMockDelegateTypes = sharpMockTypes as IAssembly;
            Unit = sharpMockTypes;

            foreach (var type in sharpMockDelegateTypes.GetAllTypes())
            {
                if (type.Name.Value == "VoidAction")
                {
                    actions.Add(type.GenericParameterCount, type);
                }

                if (type.Name.Value == "Function")
                {
                    functions.Add(type.GenericParameterCount - 1, type);
                }
            }
        }
    }
}