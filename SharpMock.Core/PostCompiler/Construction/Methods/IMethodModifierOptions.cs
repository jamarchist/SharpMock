namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface IMethodModifierOptions
    {
        IMethodBuilder Static { get; }
        IMethodBuilder Virtual { get; }
        IMethodBuilder Sealed { get; }
        //IMethodBuilder Override { get; }
    }
}