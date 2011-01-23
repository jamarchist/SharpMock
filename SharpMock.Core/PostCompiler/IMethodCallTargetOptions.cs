using System;

namespace SharpMock.PostCompiler.Core
{
    public interface IMethodCallTargetOptions
    {
        IMethodCallArgumentOptions Call(string methodName, params Type[] argumentTypes);
    }
}