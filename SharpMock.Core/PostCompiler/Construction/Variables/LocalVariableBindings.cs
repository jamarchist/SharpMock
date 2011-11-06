using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.PostCompiler.Construction.Reflection;

namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public class LocalVariableBindings : ILocalVariableBindings
    {
        private readonly IDictionary<string, IBoundExpression> bindings = new Dictionary<string, IBoundExpression>();
        private readonly IUnitReflector reflector;

        public LocalVariableBindings(IUnitReflector reflector)
        {
            this.reflector = reflector;
        }

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

        public IArrayVariableOptions<TElementType> Array<TElementType>(string arrayVariableName)
        {
            return new ArrayVariableOptions<TElementType>(arrayVariableName, this, reflector);
        }
    }
}
