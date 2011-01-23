using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public class LocalVariableBindings : ILocalVariableBindings
    {
        private readonly IDictionary<string, IBoundExpression> bindings = new Dictionary<string, IBoundExpression>();

        public void AddBinding(string localVariableName, ILocalDefinition definition, ITypeReference type)
        {
            var binding = new BoundExpression();
            binding.Definition = definition;
            binding.Type = type;

            bindings.Add(localVariableName, binding);
        }

        public IBoundExpression this[string localVariableName]
        {
            get { return bindings[localVariableName]; }
        }
    }
}
