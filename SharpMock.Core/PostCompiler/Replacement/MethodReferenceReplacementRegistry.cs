﻿using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public static class MethodReferenceReplacementRegistry
	{
        private static readonly IDictionary<IMethodReference, IMethodReference> replacements = new MethodReferenceReplacementDictionary();
        private static readonly IDictionary<IMethodReference, IMethodReference> specifications = new MethodReferenceReplacementDictionary(); 

        public static void Clear()
        {
            replacements.Clear();
        }

		public static void AddMethodToIntercept(IMethodReference method)
		{
            if (!replacements.ContainsKey(method))
            {
                replacements.Add(method, null);    
            }
		}

        public static void AddSpecification(IMethodReference method)
        {
            if (!specifications.ContainsKey(method))
            {
                specifications.Add(method, null);
            }
        }

        public static void ReplaceWith(IMethodReference original, IMethodReference replacement)
        {
            if (replacements.ContainsKey(original))
            {
                replacements[original] = replacement;
            }
        }

        public static bool HasReplacementFor(IMethodReference original)
        {
            return replacements.ContainsKey(original);
        }

        public static bool IsSpecified(IMethodReference original)
        {
            return specifications.ContainsKey(original);
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
	}
}
