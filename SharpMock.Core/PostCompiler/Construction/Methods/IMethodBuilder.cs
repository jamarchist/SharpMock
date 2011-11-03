namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface IMethodBuilder
    {
        IMethodBuilder Named(string methodName);
        IMethodBuilder WithParameters();
        IMethodBuilder WithBody();
        IMethodBuilder Returning<TReturnType>();
    }
}