using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace SharpMock.Core.Interception.Helpers
{
    public static class MatchingHelper
    {
        public static bool MethodCallMatchesAnOverload(this MethodCall call, IEnumerable<IMethodDefinition> overloads)
        {
            foreach (var overload in overloads)
            {
                var genericCall = call.MethodToCall as Microsoft.Cci.MutableCodeModel.GenericMethodInstanceReference;

                if (call.MethodToCall.Name.Value == overload.Name.Value && (call.MethodToCall.ResolvedMethod.Equals(overload.ResolvedMethod) ||
                   (genericCall != null && MemberHelper.GenericMethodSignaturesAreEqual(genericCall.GenericMethod.ResolvedMethod, overload.ResolvedMethod))))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
