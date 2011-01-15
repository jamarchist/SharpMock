namespace SharpMock.PostCompiler.Core
{
    public interface IPropertySetterTargetOptions
    {
        IPropertySetterValueOptions Set<TPropertyType>(string propertyName);
    }
}