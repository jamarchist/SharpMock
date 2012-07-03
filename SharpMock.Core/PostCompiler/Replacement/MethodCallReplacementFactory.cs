using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class MethodCallReplacementFactory : IReplacementFactory
    {
        private readonly ReturnStatement firstStatement;

        public MethodCallReplacementFactory(ReturnStatement firstStatement)
        {
            this.firstStatement = firstStatement;
        }

        public IReplacementRegistrar GetRegistrar()
        {
            return new MethodCallReplacementRegistrar();
        }

        public IReplacementBuilder GetBuilder()
        {
            return new MethodCallReplacementBuilder();
        }

        public IReplacer GetReplacer()
        {
            return new MethodCallReplacer(firstStatement);
        }
    }
}