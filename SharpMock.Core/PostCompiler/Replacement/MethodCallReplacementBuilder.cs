using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class MethodCallReplacementBuilder : IReplacementBuilder
    {
        private readonly ConstructorOrMethodCall replacementTarget;
        private readonly ReplacementRegistry registry;

        public MethodCallReplacementBuilder(ConstructorOrMethodCall replacementTarget, ReplacementRegistry registry)
        {
            this.replacementTarget = replacementTarget;
            this.registry = registry;
        }

        public object BuildReplacement()
        {
            if (replacementTarget != null)
            {
                var replaceable = replacementTarget.MethodToCall.AsReplaceable();
                if (registry.IsRegistered(replaceable))
                {
                    var replacementCall = registry.GetReplacement(replaceable);
                    replacementTarget.MethodToCall = replacementCall;

                    if (replacementTarget is CreateObjectInstance)
                    {
                        var newCall = new MethodCall();
                        newCall.Type = replacementTarget.Type;
                        newCall.Arguments = replacementTarget.Arguments;
                        newCall.Locations = replacementTarget.Locations;
                        newCall.MethodToCall = replacementCall;
                        newCall.IsStaticCall = true;

                        return newCall;
                    }
                    else
                    {
                        var call = replacementTarget as MethodCall;

                        if (!call.IsStaticCall)
                        {
                            call.Arguments.Insert(0, call.ThisArgument);
                            call.IsStaticCall = true;
                            call.IsVirtualCall = false;
                            call.ThisArgument = CodeDummy.Expression;
                        }

                        return call;
                    }
                }
            }

            return null;
        }
    }
}