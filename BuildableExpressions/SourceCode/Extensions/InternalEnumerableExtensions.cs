namespace AgileObjects.BuildableExpressions.SourceCode.Extensions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal static class InternalEnumerableExtensions
    {
#if FEATURE_READONLYDICTIONARY
        public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
#endif
        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IList<T> items)
        {
            if (items == null || items.Count == 0)
            {
                return Enumerable<T>.EmptyReadOnlyCollection;
            }

            return new ReadOnlyCollection<T>(items);
        }
    }
}
