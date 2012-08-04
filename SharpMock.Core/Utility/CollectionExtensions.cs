using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SharpMock.Core.Utility
{
    public static class CollectionExtensions
    {
        public static List<T> Where<T>(this IEnumerable<T> collection, Predicate<T> condition)
        {
            var queryableCollection = new List<T>(collection);
            return queryableCollection.FindAll(condition);
        }

        public static List<TResult> Select<T, TResult>(this IEnumerable<T> collection, Function<T, TResult> selector)
        {
            var queryableCollection = new List<T>(collection);
            var results = new List<TResult>();
            queryableCollection.ForEach(t => results.Add(selector(t)));
            return results;
        }

        public static string CommaDelimitedList<T>(this IEnumerable<T> collection)
        {
            var stringBuilder = new StringBuilder();
            var list = new List<T>(collection);
            for (var tIndex = 0; tIndex < list.Count; tIndex++)
            {
                var t = list[tIndex];

                stringBuilder.Append(t);

                if (tIndex != list.Count - 1)
                    stringBuilder.Append(',');
            }

            return stringBuilder.ToString();
        }

        public static List<T> As<T>(this IEnumerable collection) where T : class
        {
            var list = new List<T>();
            foreach (var item in collection)
            {
                list.Add(item as T);
            }

            return list;
        }
    }
}
