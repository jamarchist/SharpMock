using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class ConstructorReplacementBuilder : IReplacementBuilder
    {
        private readonly CreateObjectInstance replacementTarget;

        public ConstructorReplacementBuilder(CreateObjectInstance replacementTarget)
        {
            this.replacementTarget = replacementTarget;
        }

        public object BuildReplacement()
        {
            if (MethodReferenceReplacementRegistry.HasReplacementFor(replacementTarget.MethodToCall.AsReplaceable()))
            {
                var replacementCall =
                    MethodReferenceReplacementRegistry.GetReplacementFor(replacementTarget.MethodToCall);
                replacementTarget.MethodToCall = replacementCall;

                var newCall = new MethodCall();
                newCall.Type = replacementTarget.Type;
                newCall.Arguments = replacementTarget.Arguments;
                newCall.Locations = replacementTarget.Locations;
                newCall.MethodToCall = replacementCall;
                newCall.IsStaticCall = true;

                return newCall;
            }

            return null;
        }
    }
}