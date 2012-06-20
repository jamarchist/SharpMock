using System.Collections.Generic;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class ReplaceableMethodInfoComparer : EqualityComparer<ReplaceableMethodInfo>
    {
        public override bool Equals(ReplaceableMethodInfo x, ReplaceableMethodInfo y)
        {
            if (    x.Name == y.Name 
                    &&  x.DeclaringType.Namespace == y.DeclaringType.Namespace
                    &&  x.DeclaringType.Name == y.DeclaringType.Name
                    &&  x.DeclaringType.Assembly.Name == y.DeclaringType.Assembly.Name)
            {
                return true;
            }

            return false;            
        }

        public override int GetHashCode(ReplaceableMethodInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}