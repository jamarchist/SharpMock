using System;
using Microsoft.Cci;

namespace SharpMock.PostCompiler.Core.CodeConstruction
{
    public interface IMethodCallBuilder
    {
        IMethodCallArgumentOptions Method(string methodName, params Type[] argumentTypes);
        IMethodCallArgumentOptions VirtualMethod(string methodName, params Type[] argumentTypes);
        IMethodCallArgumentOptions StaticMethod(string methodName, params Type[] argumentTypes);

        IMethodCallArgumentOptions Method(IMethodReference method);
        IMethodCallArgumentOptions VirtualMethod(IMethodReference method);
        IMethodCallArgumentOptions StaticMethod(IMethodReference method);

        IMethodCallOptions PropertySetter<TPropertyType>(string propertyName);
        IMethodCallReturnOptions PropertyGetter<TPropertyType>(string propertyName);
        IMethodCallOptions StaticPropertySetter<TPropertyType>(string propertyName);
        IMethodCallReturnOptions StaticPropertyGetter<TPropertyType>(string propertyName);
        IMethodCallOptions VirtualPropertySetter<TPropertyType>(string propertyName);
        IMethodCallReturnOptions VirtualPropertyGetter<TPropertyType>(string propertyName);
    }
}
