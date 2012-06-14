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
                    &&  x.DeclaringType.Assembly.AssemblyFullName == y.DeclaringType.Assembly.AssemblyFullName
                    &&  x.DeclaringType.Assembly.AssemblyPath == y.DeclaringType.Assembly.AssemblyPath)
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