using System.Collections;
using System.Collections.Generic;
using Microsoft.Cci;

namespace SharpMock.PostCompiler
{
    public class MethodReferenceReplacementDictionary : IDictionary<IMethodReference, IMethodReference>
    {
        private readonly Dictionary<IMethodReference, IMethodReference> inner = new Dictionary<IMethodReference, IMethodReference>();

        public IEnumerator<KeyValuePair<IMethodReference, IMethodReference>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<IMethodReference, IMethodReference> item)
        {
            inner.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            inner.Clear();
        }

        public bool Contains(KeyValuePair<IMethodReference, IMethodReference> item)
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

        public void CopyTo(KeyValuePair<IMethodReference, IMethodReference>[] array, int arrayIndex)
        {
            new List<KeyValuePair<IMethodReference, IMethodReference>>(inner).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<IMethodReference, IMethodReference> item)
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

        public bool ContainsKey(IMethodReference key)
        {
            var matchingKey = GetMatchingKey(key);
            return matchingKey != null;
        }

        private IMethodReference GetMatchingKey(IMethodReference key)
        {
            var allKeys = new List<IMethodReference>(inner.Keys);
            return allKeys.Find(mr => mr.ResolvedMethod.Equals(key.ResolvedMethod));
        }

        public void Add(IMethodReference key, IMethodReference value)
        {
            inner.Add(key, value);
        }

        public bool Remove(IMethodReference key)
        {
            return inner.Remove(key);
        }

        public bool TryGetValue(IMethodReference key, out IMethodReference value)
        {
            return inner.TryGetValue(key, out value);
        }

        public IMethodReference this[IMethodReference key]
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

        public ICollection<IMethodReference> Keys
        {
            get { return inner.Keys; }
        }

        public ICollection<IMethodReference> Values
        {
            get { return inner.Values; }
        }
    }
}