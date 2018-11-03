
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class IEnumerableOfTExtensions
    {
        public static IEnumerable<ValueTuple<TSource, TSource>> FullOuterJoin<TSource>(this IEnumerable<TSource> items1, IEnumerable<TSource> items2, IEqualityComparer<TSource> comparer)
        {
            var items = items1.Union(items2, comparer);

            foreach (var item in items)
            {
                var item1 = items1.SingleOrDefault(i => comparer.Equals(i, item));
                var item2 = items2.SingleOrDefault(i => comparer.Equals(i, item));

                yield return (item1, item2);
            }
        }
    }
}
