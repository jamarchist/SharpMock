using System;
using System.Collections.Generic;
using Microsoft.Cci;
using SharpMock.Core.Diagnostics;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.Utility;

namespace SharpMock.Core.PostCompiler.Replacement
{
    [Serializable]
    public class ReplacementRegistry
    {
        private readonly IDictionary<string, IDictionary<IReplaceableReference, IMethodReference>> registries = 
            new Dictionary<string, IDictionary<IReplaceableReference, IMethodReference>>();
        [NonSerialized]
        private readonly ILogger log;

        public ReplacementRegistry(ILogger log)
        {
            this.log = log;
            InitializeRegistries();
        }

        private void InitializeRegistries()
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
                log.WriteTrace("Registering '{0}'.", TraceHelper.GetDebuggerDisplay(reference));
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

        public void Load(ReplaceableCodeInfo specifications)
        {
            Clear();
            RegisterAll(specifications.Methods.As<IReplaceableReference>());
            RegisterAll(specifications.FieldAccessors.As<IReplaceableReference>());
            RegisterAll(specifications.FieldAssignments.As<IReplaceableReference>());
        }

        private void Clear()
        {
            foreach (var registry in registries.Values)
            {
                registry.Clear();
            }            
        }

        private void RegisterAll(IEnumerable<IReplaceableReference> references)
        {
            foreach (var reference in references)
            {
                RegisterReference(reference);
            }            
        }
    }
}