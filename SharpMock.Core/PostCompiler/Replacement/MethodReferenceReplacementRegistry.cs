using System.Collections.Generic;
using Microsoft.Cci;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public static class MethodReferenceReplacementRegistry
	{
        private static readonly IList<ReplaceableMethodInfo> replaceables = new List<ReplaceableMethodInfo>();  
        private static readonly IDictionary<IMethodReference, IMethodReference> replacements = new MethodReferenceReplacementDictionary();

        public static void AddMethodToIntercept(IMethodReference method)
		{
            if (!replacements.ContainsKey(method))
            {
                replaceables.Add(method.AsReplaceable());
                replacements.Add(method, null);    
            }
		}

        public static void ReplaceWith(IMethodReference original, IMethodReference replacement)
        {
            if (replacements.ContainsKey(original))
            {
                replacements[original] = replacement;
            }
        }

        public static bool HasReplacementFor(ReplaceableMethodInfo methodInfo)
        {
            var list = new List<ReplaceableMethodInfo>(replaceables);
            return list.FindAll(r => new ReplaceableMethodInfoComparer().Equals(methodInfo, r)).Count > 0;
        }

        public static IMethodReference GetReplacementFor(IMethodReference original)
        {
            if (replacements.ContainsKey(original))
            {
                return replacements[original];
            }

            return null;
        }

		public static IEnumerable<IMethodReference> GetMethodsToIntercept()
		{
		    var keys = new IMethodReference[replacements.Keys.Count];
            replacements.Keys.CopyTo(keys, 0);
		    return keys;
		}

        public static IList<ReplaceableMethodInfo> GetReplaceables()
        {
            return replaceables;
        }
	}
}
