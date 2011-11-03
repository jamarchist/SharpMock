namespace SharpMock.Core.PostCompiler.Construction.Methods
{
    public interface IMethodAccessibilityOptions
    {
        IMethodModifierOptions Public { get; }
        IMethodModifierOptions Private { get; }
        IMethodModifierOptions Internal { get; }
        IMethodModifierOptions Protected { get; }
        IMethodModifierOptions ProtectedInternal { get; }
    }
}