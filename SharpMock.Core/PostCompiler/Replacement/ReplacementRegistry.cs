using System.Collections.Generic;
using Microsoft.Cci;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplacementRegistry
    {
        private readonly IDictionary<string, IDictionary<IReplaceableReference, IMethodReference>> registries = 
            new Dictionary<string, IDictionary<IReplaceableReference, IMethodReference>>();

        public ReplacementRegistry()
        {
            registries.Add(ReplaceableReferenceTypes.Method, new Dictionary<IReplaceableReference, IMethodReference>());
            registries.Add(ReplaceableReferenceTypes.FieldAccessor, new Dictionary<IReplaceableReference, IMethodReference>());
            registries.Add(ReplaceableReferenceTypes.FieldAssignment, new Dictionary<IReplaceableReference, IMethodReference>());
        }

        public void RegisterReference(IReplaceableReference reference)
        {
            var registry = registries[reference.ReferenceType];
            if (!registry.ContainsKey(reference))
            {
                registry.Add(reference, null);    
            }
        }

        public void RegisterReplacement(IReplaceableReference originalReference, IMethodReference replacement)
        {
            registries[originalReference.ReferenceType][originalReference] = replacement;
        }

        public bool IsRegistered(IReplaceableReference reference)
        {
            return registries[reference.ReferenceType].ContainsKey(reference);
        }

        public IMethodReference GetReplacement(IReplaceableReference reference)
        {
            return registries[reference.ReferenceType][reference];
        }

        public List<IReplaceableReference> GetRegisteredReferences(string referenceType)
        {
            return new List<IReplaceableReference>(registries[referenceType].Keys);
        }
    }
}