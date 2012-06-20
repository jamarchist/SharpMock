using System.Collections.Generic;
using System.Reflection;

namespace SharpMock.Core.Interception.MatchingStrategies
{
    public class MethodInfoComparer : EqualityComparer<MethodInfo>
    {
        public override bool Equals(MethodInfo x, MethodInfo y)
        {
            // if 'basic' equality fails, we can short-circuit
            // otherwise we assume method name and parent type
            // are the same and we need to check arguments
            if (!x.Equals(y))
            {
                return false;
            }

            var xParams = x.GetParameters();
            var yParams = y.GetParameters();

            if (!xParams.Length.Equals(yParams.Length))
            {
                return false;
            }

            for (int parameterIndex = 0; parameterIndex < xParams.Length; parameterIndex++)
            {
                if (!xParams[parameterIndex].ParameterType
                         .Equals(yParams[parameterIndex].ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode(MethodInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}