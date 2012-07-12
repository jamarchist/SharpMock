using System;
using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Construction.Reflection
{
    public class SharpMockTypes
    {
        public interface IGenericMethodTypeDictionary { INamedTypeReference this[int numberOfParameters] { get; } }

        private readonly IGenericMethodTypeDictionary functions;
        private readonly IGenericMethodTypeDictionary actions;

        public IGenericMethodTypeDictionary Functions { get { return functions; } }
        public IGenericMethodTypeDictionary Actions { get { return actions; } }

        public IUnit Unit { get; private set; }
        
        public SharpMockTypes(IMetadataHost host)
        {
            var funcs = new Dictionary<int, INamedTypeReference>();
            var acts = new Dictionary<int, INamedTypeReference>();

            var sharpMockTypes =
                host.LoadUnitFrom(
                    System.Reflection.Assembly.GetExecutingAssembly().Location);
            var sharpMockDelegateTypes = sharpMockTypes as IAssembly;
            Unit = sharpMockTypes;

            foreach (var type in sharpMockDelegateTypes.GetAllTypes())
            {
                if (type.Name.Value == "VoidAction")
                {
                    acts.Add(type.GenericParameterCount, type);
                }

                if (type.Name.Value == "Function")
                {
                    funcs.Add(type.GenericParameterCount - 1, type);
                }
            }

            functions = new GenericMethodTypeDictionary(funcs, "Unable to find type Function<> with {0} input parameter arguments.");
            actions = new GenericMethodTypeDictionary(acts, "Unable to find type VoidAction<> with {0} parameter arguments.");
        }

        private class GenericMethodTypeDictionary : IGenericMethodTypeDictionary
        {
            private readonly IDictionary<int, INamedTypeReference> types;
            private readonly string errorMessageFormat;

            public GenericMethodTypeDictionary(IDictionary<int, INamedTypeReference> types, string errorMessageFormat)
            {
                this.types = types;
                this.errorMessageFormat = errorMessageFormat;
            }

            public INamedTypeReference this[int numberOfParameters]
            {
                get
                {
                    try
                    {
                        return types[numberOfParameters];
                    }
                    catch (KeyNotFoundException keyNotFound)
                    {
                        throw new InvalidOperationException(String.Format(errorMessageFormat, numberOfParameters), keyNotFound);
                    }
                }
            }
        }
    }
}