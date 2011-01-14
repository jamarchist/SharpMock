using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.PostCompiler.Core
{
    public interface IDefinitionBuilder
    {
        LocalDefinition VariableOf<TVariableType>(string variableName);
        LocalDefinition VariableOf(string variableName, ITypeReference type);
    }

    public class DefinitionBuilder : IDefinitionBuilder
    {
        private readonly IUnitReflector reflector;
        private readonly ILocalVariableBindings bindings;
        private readonly INameTable nameTable;

        public DefinitionBuilder(IUnitReflector reflector, ILocalVariableBindings bindings, INameTable nameTable)
        {
            this.reflector = reflector;
            this.bindings = bindings;
            this.nameTable = nameTable;
        }

        public LocalDefinition VariableOf<TVariableType>(string variableName)
        {
            var type = reflector.Get<TVariableType>().ResolvedType;
            return VariableOf(variableName, type);
        }

        public LocalDefinition VariableOf(string variableName, ITypeReference type)
        {
            var local = new LocalDefinition();
            local.Name = nameTable.GetNameFor(variableName);
            local.Type = type;

            bindings.AddBinding(variableName, local, local.Type);

            return local;
        }
    }
}
