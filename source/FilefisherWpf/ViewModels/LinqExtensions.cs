using System;
using System.Collections.Generic;
using System.Linq;

namespace FilefisherWpf.ViewModels
{
    public static class LinqExtensions
    {
        public static IEnumerable<TItem> DistinctBy<TItem, TKey>(this IEnumerable<TItem> items,
            Func<TItem, TKey> keySelector)
        {
            return items.Distinct(new SelectorEqualityComparer<TItem, TKey>(keySelector));
        }

        /// <summary>
        ///     Filters a collection of items by another set using a specified comparison key selector.
        /// </summary>
        public static IEnumerable<TItem> ExceptBy<TItem, TKey>(this IEnumerable<TItem> items,
            IEnumerable<TItem> excludeSet, Func<TItem, TKey> keySelector)
        {
            return items.Except(excludeSet, new SelectorEqualityComparer<TItem, TKey>(keySelector));
        }

        /// <summary>
        ///     Filters a collection of items by a predicate. Same as negation of Where.
        /// </summary>
        public static IEnumerable<TItem> Except<TItem>(this IEnumerable<TItem> items, Predicate<TItem> predicate)
        {
            return items.Where(x => !predicate(x));
        }
    }
}