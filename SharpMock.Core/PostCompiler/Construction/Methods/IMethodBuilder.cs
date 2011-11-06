using System;
using System.Collections.Generic;

namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface IMethodBuilder
    {
        IMethodBuilder Named(string methodName);
        IMethodBuilder WithParameters(Dictionary<string, Type> parameters);
        IMethodBuilder WithBody(VoidAction<ICodeBuilder> code);
        IMethodBuilder Returning<TReturnType>();
    }
}