namespace SharpMock.Core.PostCompiler.Construction.Classes
{
    public interface IClassAccessiblityOptions 
    {
        IClassModifierOptions Public { get; }
        IClassModifierOptions Private { get; }
        IClassModifierOptions Internal { get; }
    }
}