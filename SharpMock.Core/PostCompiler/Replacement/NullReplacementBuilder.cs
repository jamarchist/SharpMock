namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class NullReplacementBuilder : IReplacementBuilder
    {
        public object BuildReplacement()
        {
            return new object();
        }
    }
}