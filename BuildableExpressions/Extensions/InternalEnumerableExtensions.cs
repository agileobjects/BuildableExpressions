namespace AgileObjects.BuildableExpressions.Extensions
{
    using System;
    using System.Collections.Generic;
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
    }
}
