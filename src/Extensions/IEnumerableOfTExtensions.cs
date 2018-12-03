
namespace FolderCompare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class IEnumerableOfTExtensions
    {
        public static IEnumerable<IGrouping<TKey, TSource>> GetDuplicates<TSource, TKey>(this IEnumerable<TSource> items, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var query = items.GroupBy(keySelector, comparer)
                        .Where(g => g.Count() > 1);

            return query;
        }

        public static IEnumerable<TSource> RemoveDuplicates<TSource, TKey>(this IEnumerable<TSource> items, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var query = items.GroupBy(keySelector, comparer)
                        .Where(g => g.Count() == 1)
                        .Select(g =>g.First());

            return query;
        }
    }
}
