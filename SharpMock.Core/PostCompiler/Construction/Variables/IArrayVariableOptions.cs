namespace SharpMock.Core.PostCompiler.Construction.Variables
{
    public interface IArrayVariableOptions<TElementType>
    {
        IArrayIndexerOptions<TElementType> this[int index] { get; }
    }
}