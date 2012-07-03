using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

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
                var replaceable = target.MethodToCall.AsReplaceable();
                MethodReferenceReplacementRegistry.AddReplaceable(replaceable);
                MethodReferenceReplacementRegistry.AddMethodToIntercept(target.MethodToCall);
            }
        }
    }
}