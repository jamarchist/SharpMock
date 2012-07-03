namespace SharpMock.Core.PostCompiler.Replacement
{
    internal interface IReplacementFactory
    {
        IReplacementRegistrar GetRegistrar();
        IReplacementBuilder GetBuilder();
        IReplacer GetReplacer();
    }
}