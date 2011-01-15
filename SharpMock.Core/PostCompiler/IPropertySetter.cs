using System;

namespace SharpMock.PostCompiler.Core
{
    public interface IPropertySetter
    {
        IPropertySetterTargetOptions On<TTargetType>(string variableName);
    }
}

