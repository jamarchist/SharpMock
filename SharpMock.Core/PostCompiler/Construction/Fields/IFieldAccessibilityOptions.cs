namespace SharpMock.Core.PostCompiler.Construction.Fields
{
    public interface IFieldAccessibilityOptions
    {
        IFieldModifierOptions Public { get; }
        IFieldModifierOptions Private { get; }
        IFieldModifierOptions Internal { get; }
        IFieldModifierOptions Protected { get; }
        IFieldModifierOptions ProtectedInternal { get; }        
    }
}