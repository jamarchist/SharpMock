using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public class ParameterBindings : IParameterBindings
    {
        private readonly IDictionary<string, IBoundExpression> bindings = new Dictionary<string, IBoundExpression>();
        
        public void AddBinding(IParameterDefinition definition)
        {
            var binding = new BoundExpression();
            binding.Definition = definition;
            binding.Type = definition.Type;

            bindings.Add(definition.Name.Value, binding);
        }

        public IBoundExpression this[string parameterName]
        {
            get { return bindings[parameterName]; }
        }
    }
}