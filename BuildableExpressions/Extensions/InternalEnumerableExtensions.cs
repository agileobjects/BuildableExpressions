namespace AgileObjects.BuildableExpressions.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using SourceCode.Extensions;

    internal static class InternalEnumerableExtensions
    {
        public static TResult[] ProjectToArray<TItem, TResult>(
            this IList<TItem> items,
            Func<TItem, TResult> projector)
        {
            var itemCount = items.Count;

            switch (itemCount)
            {
                case 0:
                    return Enumerable<TResult>.EmptyArray;

                case 1:
                    return new[] { projector.Invoke(items[0]) };

                default:

                    var result = new TResult[itemCount];

                    for (var i = 0; i < itemCount; ++i)
                    {
                        result[i] = projector.Invoke(items[i]);
                    }

                    return result;
            }
        }

        [DebuggerStepThrough]
        public static IEnumerable<TResult> Project<TItem, TResult>(
            this IEnumerable<TItem> items,
            Func<TItem, TResult> projector)
        {
            foreach (var item in items)
            {
                yield return projector.Invoke(item);
            }
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            foreach (var item in items)
            {
                if (predicate.Invoke(item))
                {
                    yield return item;
                }
            }
        }
    }
}
