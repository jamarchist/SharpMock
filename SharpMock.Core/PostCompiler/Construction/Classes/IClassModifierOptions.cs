namespace SharpMock.Core.PostCompiler.Construction.Classes
{
    public interface IClassModifierOptions
    {
        IClassBuilder Static { get; }
        IClassBuilder Abstract { get; }
        IClassBuilder Concrete { get; }
    }
}