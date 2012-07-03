using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class MethodCallReplacementFactory : IReplacementFactory
    {
        private readonly IStatement firstStatement;

        public MethodCallReplacementFactory(IStatement firstStatement)
        {
            this.firstStatement = firstStatement;
        }

        public IReplacementRegistrar GetRegistrar()
        {
            if (firstStatement is ReturnStatement)
                return new MethodCallReplacementRegistrar((firstStatement as ReturnStatement).Expression as ConstructorOrMethodCall);
            else
                return new MethodCallReplacementRegistrar((firstStatement as ExpressionStatement).Expression as ConstructorOrMethodCall);
        }

        public IReplacementBuilder GetBuilder()
        {
            if (firstStatement is ReturnStatement)
                return new MethodCallReplacementBuilder((firstStatement as ReturnStatement).Expression as ConstructorOrMethodCall);
            else
                return new MethodCallReplacementBuilder((firstStatement as ExpressionStatement).Expression as ConstructorOrMethodCall);
        }

        public IReplacer GetReplacer()
        {
            return new NullReplacer();
        }
    }
}