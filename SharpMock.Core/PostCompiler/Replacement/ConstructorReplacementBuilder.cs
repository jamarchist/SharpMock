using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class ConstructorReplacementBuilder : IReplacementBuilder
    {
        private readonly CreateObjectInstance replacementTarget;
        private readonly ReplacementRegistry registry;

        public ConstructorReplacementBuilder(CreateObjectInstance replacementTarget, ReplacementRegistry registry)
        {
            this.replacementTarget = replacementTarget;
            this.registry = registry;
        }

        public object BuildReplacement()
        {
            var replaceableMethod = replacementTarget.MethodToCall.AsReplaceable();

            if (registry.IsRegistered(replaceableMethod))
            {
                var replacementMethodReference = registry.GetReplacement(replaceableMethod);
                //var replacementCall =
                //    MethodReferenceReplacementRegistry.GetReplacementFor(replacementTarget.MethodToCall);
                replacementTarget.MethodToCall = replacementMethodReference;

                var newCall = new MethodCall();
                newCall.Type = replacementTarget.Type;
                newCall.Arguments = replacementTarget.Arguments;
                newCall.Locations = replacementTarget.Locations;
                newCall.MethodToCall = replacementMethodReference;
                newCall.IsStaticCall = true;

                return newCall;
            }

            return null;
        }
    }
}