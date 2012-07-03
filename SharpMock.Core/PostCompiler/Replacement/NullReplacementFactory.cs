namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class NullReplacementFactory : IReplacementFactory
    {
        public IReplacementRegistrar GetRegistrar()
        {
            return new NullReplacementRegistrar();
        }

        public IReplacementBuilder GetBuilder()
        {
            return new NullReplacementBuilder();
        }

        public IReplacer GetReplacer()
        {
            return new NullReplacer();
        }
    }
}