using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class MethodCallReplacementRegistrar : IReplacementRegistrar
    {
        public void RegisterReplacement(object replacementTarget)
        {
            var firstMethodCall = replacementTarget as ConstructorOrMethodCall;

            if (firstMethodCall != null)
            {
                var replaceable = firstMethodCall.MethodToCall.AsReplaceable();
                MethodReferenceReplacementRegistry.AddReplaceable(replaceable);
                MethodReferenceReplacementRegistry.AddMethodToIntercept(firstMethodCall.MethodToCall);
            }
        }
    }
}