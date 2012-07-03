using System.Collections.Generic;
using Microsoft.Cci;
using SharpMock.Core.Interception.Registration;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public static class FieldReferenceReplacementRegistry
    {
        private static readonly IList<ReplaceableFieldAccessorInfo> replaceables = new List<ReplaceableFieldAccessorInfo>(); 
        private static readonly IDictionary<IFieldReference, IMethodReference> replacements = new Dictionary<IFieldReference, IMethodReference>();

        public static void AddFieldToIntercept(IFieldReference field)
        {
            if (!replacements.ContainsKey(field))
            {
                replaceables.Add(field.AsReplaceable());
                replacements.Add(field, null);
            }
        }

        public static void ReplaceWith(IFieldReference original, IMethodReference replacement)
        {
            if (replacements.ContainsKey(original))
            {
                replacements[original] = replacement;
            }
        }

        public static bool HasReplacementFor(ReplaceableFieldAccessorInfo fieldInfo)
        {
            var list = new List<ReplaceableFieldAccessorInfo>(replaceables);
            return list.FindAll(r => new ReplaceableFieldAccessorInfoComparer().Equals(fieldInfo, r)).Count > 0;            
        }

        public static IMethodReference GetReplacementFor(IFieldReference original)
        {
            if (replacements.ContainsKey(original))
            {
                return replacements[original];
            }

            return null;
        }

        public static IEnumerable<IFieldReference> GetFieldsToIntercept()
        {
            var keys = new IFieldReference[replacements.Keys.Count];
            replacements.Keys.CopyTo(keys, 0);
            return keys;
        }

        public static IList<ReplaceableFieldAccessorInfo> GetReplaceables()
        {
            return replaceables;
        }
    }
}