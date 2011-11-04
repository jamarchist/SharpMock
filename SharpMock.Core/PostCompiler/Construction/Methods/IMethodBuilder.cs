namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface IMethodBuilder
    {
        IMethodBuilder Named(string methodName);
        IMethodBuilder WithParameters();
        IMethodBuilder WithBody(VoidAction<ICodeBuilder> code);
        IMethodBuilder Returning<TReturnType>();
    }
}