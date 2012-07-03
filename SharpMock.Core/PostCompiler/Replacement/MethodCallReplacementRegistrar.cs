using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class MethodCallReplacementRegistrar : IReplacementRegistrar
    {
        private readonly ConstructorOrMethodCall target;

        public MethodCallReplacementRegistrar(ConstructorOrMethodCall target)
        {
            this.target = target;
        }

        public void RegisterReplacement()
        {
            if (target != null)
            {
                MethodReferenceReplacementRegistry.AddMethodToIntercept(target.MethodToCall);
            }
        }
    }
}