using Microsoft.Cci;
using SharpMock.Core.PostCompiler.Construction.Reflection;
using SharpMock.Core.PostCompiler.Construction.Variables;
using SharpMock.PostCompiler.Core;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public class PropertySetterTargetOptions : IPropertySetterTargetOptions
    {
        private readonly ITypeDefinitionExtensions typeDefinitionExtensions;
        private readonly IBoundExpression variable;
        private readonly IMetadataHost host;
        private readonly ILocalVariableBindings locals;

        public PropertySetterTargetOptions(ITypeDefinitionExtensions typeDefinitionExtensions, IBoundExpression variable, IMetadataHost host, ILocalVariableBindings locals)
        {
            this.typeDefinitionExtensions = typeDefinitionExtensions;
            this.locals = locals;
            this.host = host;
            this.variable = variable;
        }

        public IPropertySetterValueOptions Set<TPropertyType>(string propertyName)
        {
            var setter = typeDefinitionExtensions.GetPropertySetter<TPropertyType>(propertyName);
            return new PropertySetterValueOptions(setter, variable, host, locals);
        }
    }
}