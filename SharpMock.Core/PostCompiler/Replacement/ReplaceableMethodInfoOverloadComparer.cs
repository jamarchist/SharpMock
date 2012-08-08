using System.Collections.Generic;
using SharpMock.Core.Interception.Registration;
using SharpMock.Core.Utility;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplaceableMethodInfoOverloadComparer : EqualityComparer<ReplaceableMethodInfo>
    {
        public override bool Equals(ReplaceableMethodInfo x, ReplaceableMethodInfo y)
        {
            return Generate.Equality(x, y, m => 
                m.Name, m => m.DeclaringType.Namespace, m => m.DeclaringType.Name, m => m.DeclaringType.Assembly.Name);
        }

        public override int GetHashCode(ReplaceableMethodInfo obj)
        {
            return Generate.HashCode(71, 23, obj.Name, obj.DeclaringType.Namespace, obj.DeclaringType.Name, obj.DeclaringType.Assembly.Name);
        }
    }
}