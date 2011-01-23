using SharpMock.PostCompiler.Core;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public interface IPropertySetterTargetOptions
    {
        IPropertySetterValueOptions Set<TPropertyType>(string propertyName);
    }
}