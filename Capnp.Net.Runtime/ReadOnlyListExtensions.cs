using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable
namespace Capnp
{
    /// <summary>
    /// Provides extension methods for <see cref="IReadOnlyList{T}"/>
    /// </summary>
    public static class ReadOnlyListExtensions
    {
        class ReadOnlyListSelectOperator<From, To> : IReadOnlyList<To>
        {
            readonly IReadOnlyList<From> _source;
            readonly Func<From, To> _selector;

            public ReadOnlyListSelectOperator(IReadOnlyList<From> source, Func<From, To> selector)
            {
                _source = source ?? throw new ArgumentNullException(nameof(source));
                _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            }

            public To this[int index] => _selector(_source[index]);

            public int Count => _source.Count;

            public IEnumerator<To> GetEnumerator()
            {
                return _source.Select(_selector).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// LINQ-like "Select" operator for <see cref="IReadOnlyList{T}"/>, with the addition that the resulting elements are accessible by index.
        /// The operator implements lazy semantics, which means that the selector function results are not cached./>
        /// </summary>
        /// <typeparam name="From">Source element type</typeparam>
        /// <typeparam name="To">Target element type</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="selector">Selector function</param>
        /// <returns>A read-only list in which each element corresponds to the source element after applying the selector function</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static IReadOnlyList<To> LazyListSelect<From, To>(
            this IReadOnlyList<From> source, Func<From, To> selector)
        {
            return new ReadOnlyListSelectOperator<From, To>(source, selector);
        }

        /// <summary>
        /// Applies a selector function to each list element and stores the result in a new list.
        /// As opposed to <see cref="LazyListSelect{From, To}(IReadOnlyList{From}, Func{From, To})"/> the source is evaluated immediately
        /// and the result is cached.
        /// </summary>
        /// <typeparam name="From">Source element type</typeparam>
        /// <typeparam name="To">Target element type</typeparam>
        /// <param name="source">Source list</param>
        /// <param name="selector">Selector function</param>
        /// <returns>A read-only list in which each element corresponds to the source element after applying the selector function</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
        public static IReadOnlyList<To> ToReadOnlyList<From, To>(
            this IReadOnlyList<From> source, Func<From, To> selector)
        {
            return source.Select(selector).ToList().AsReadOnly();
        }
    }
}
#nullable restore