using System;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Construction.Expressions
{
    public interface IMethodCallReturnOptions
    {
        MethodCall On(string localVariableName);
        MethodCall On(ITypeReference type);
        MethodCall On<TTypeWithStaticMethod>();
        MethodCall On(Type staticType);
    }
}