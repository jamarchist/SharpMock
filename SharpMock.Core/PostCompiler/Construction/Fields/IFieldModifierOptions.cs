namespace SharpMock.Core.PostCompiler.Construction.Fields
{
    public interface IFieldModifierOptions
    {
        IFieldBuilder Static { get; }
        IFieldBuilder Instance { get; }
        IFieldModifierOptions Readonly { get; }
    }
}