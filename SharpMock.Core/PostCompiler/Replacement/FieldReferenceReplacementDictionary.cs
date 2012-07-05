using System.Collections;
using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.Core.PostCompiler.Replacement
{
    public class FieldReferenceReplacementDictionary : IDictionary<IFieldReference, IMethodReference>
    {
        private readonly Dictionary<IFieldReference, IMethodReference> inner = new Dictionary<IFieldReference, IMethodReference>();

        public IEnumerator<KeyValuePair<IFieldReference, IMethodReference>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<IFieldReference, IMethodReference> item)
        {
            inner.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public bool Contains(KeyValuePair<IFieldReference, IMethodReference> item)
        {
            if (!ContainsKey(item.Key))
            {
                return false;
            }

            var value = inner[item.Key];
            if (!value.Equals(item.Value))
            {
                return false;
            }

            return true;
        }

        public void CopyTo(KeyValuePair<IFieldReference, IMethodReference>[] array, int arrayIndex)
        {
            new List<KeyValuePair<IFieldReference, IMethodReference>>(inner).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<IFieldReference, IMethodReference> item)
        {
            if (Contains(item))
            {
                return inner.Remove(item.Key);
            }

            return false;
        }

        public int Count
        {
            get { return inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool ContainsKey(IFieldReference key)
        {
            var matchingKey = GetMatchingKey(key);
            return matchingKey != null;
        }

        private IFieldReference GetMatchingKey(IFieldReference key)
        {
            var allKeys = new List<IFieldReference>(inner.Keys);
            return allKeys.Find(mr => mr.ResolvedField.Equals(key.ResolvedField));
        }

        public void Add(IFieldReference key, IMethodReference value)
        {
            inner.Add(key, value);
        }

        public bool Remove(IFieldReference key)
        {
            return inner.Remove(key);
        }

        public bool TryGetValue(IFieldReference key, out IMethodReference value)
        {
            return inner.TryGetValue(key, out value);
        }

        public IMethodReference this[IFieldReference key]
        {
            get
            {
                var matchingKey = GetMatchingKey(key);
                if (matchingKey != null)
                {
                    return inner[matchingKey];
                }
                // This will actually throw an exception
                // I leave it in to preserve the exception semantics
                return inner[key];
            }
            set { inner[key] = value; }
        }

        public ICollection<IFieldReference> Keys
        {
            get { return inner.Keys; }
        }

        public ICollection<IMethodReference> Values
        {
            get { return inner.Values; }
        }        
    }
}