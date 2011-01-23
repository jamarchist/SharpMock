using SharpMock.PostCompiler.Core;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public interface IPropertySetter
    {
        IPropertySetterTargetOptions On<TTargetType>(string variableName);
    }
}

