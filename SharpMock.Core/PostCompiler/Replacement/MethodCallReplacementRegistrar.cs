using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class MethodCallReplacementRegistrar : IReplacementRegistrar
    {
        private readonly ConstructorOrMethodCall target;
        private readonly ReplacementRegistry registry;

        public MethodCallReplacementRegistrar(ConstructorOrMethodCall target, ReplacementRegistry registry)
        {
            this.target = target;
            this.registry = registry;
        }

        public void RegisterReplacement()
        {
            if (target != null)
            {
                registry.RegisterReference(target.MethodToCall.AsReplaceable());
                //MethodReferenceReplacementRegistry.AddMethodToIntercept(target.MethodToCall);
            }
        }
    }
}