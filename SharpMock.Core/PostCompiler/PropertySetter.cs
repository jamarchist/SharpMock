using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core
{
    public class PropertySetter : IPropertySetter
    {
        private readonly ILocalVariableBindings locals;
        private readonly IUnitReflector reflector;
        private readonly IMetadataHost host;

        public PropertySetter(ILocalVariableBindings locals, IUnitReflector reflector, IMetadataHost host)
        {
            this.locals = locals;
            this.host = host;
            this.reflector = reflector;
        }

        public IPropertySetterTargetOptions On<TTargetType>(string variableName)
        {
            var typeDefinitionExtensions = reflector.From<TTargetType>();
            var variable = locals[variableName];

            return new PropertySetterTargetOptions(typeDefinitionExtensions, variable, host, locals);
        }
    }
}