using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    /// <summary>
    /// Some useful extensions for the <c>IEnumerable</c> types.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Tests weather the given enumerable is empty.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable values</typeparam>
        /// <param name="enumerable">The enumerable to test</param>
        /// <returns><c>true</c> if and only if <see cref="enumerable"/> has no items.</returns>
        [Pure] public static bool IsEmptyEnumerable<T>(this IEnumerable<T> enumerable)
        {
            Contract.Requires(enumerable != null);

            return !enumerable.GetEnumerator().MoveNext();
        }
    }
}
