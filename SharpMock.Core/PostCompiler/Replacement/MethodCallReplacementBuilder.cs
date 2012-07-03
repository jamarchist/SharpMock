using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    internal class MethodCallReplacementBuilder : IReplacementBuilder
    {
        public object BuildReplacement(object replacementTarget)
        {
            var firstMethodCall = replacementTarget as ConstructorOrMethodCall;

            if (firstMethodCall != null)
            {
                if (MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall.AsReplaceable()))
                    //if (MethodReferenceReplacementRegistry.HasReplacementFor(firstMethodCall.MethodToCall))
                {
                    var replacementCall =
                        MethodReferenceReplacementRegistry.GetReplacementFor(firstMethodCall.MethodToCall);
                    firstMethodCall.MethodToCall = replacementCall;

                    if (firstMethodCall is CreateObjectInstance)
                    {
                        var newCall = new MethodCall();
                        newCall.Type = firstMethodCall.Type;
                        newCall.Arguments = firstMethodCall.Arguments;
                        newCall.Locations = firstMethodCall.Locations;
                        newCall.MethodToCall = replacementCall;
                        newCall.IsStaticCall = true;

                        return newCall;
                    }
                    else
                    {
                        var call = firstMethodCall as MethodCall;

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