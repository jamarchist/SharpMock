using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;
using SharpMock.PostCompiler.Core;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
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