using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class MethodCallReplacer : IReplacer
    {
        private readonly ReturnStatement firstStatement;

        public MethodCallReplacer(ReturnStatement firstStatement)
        {
            this.firstStatement = firstStatement;
        }

        public void ReplaceWith(object replacement)
        {
            if (firstStatement != null && firstStatement.Expression is CreateObjectInstance)
            {
                firstStatement.Expression = replacement as ConstructorOrMethodCall;    
            }
        }
    }
}