namespace SharpMock.PostCompiler.Core
{
    public interface IMethodCaller
    {
        IMethodCallTargetOptions On<TTargetType>();
    }
}