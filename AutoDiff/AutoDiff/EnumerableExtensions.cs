using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace AutoDiff
{
    public static class EnumerableExtensions
    {
        [Pure] public static bool IsEmptyEnumerable<T>(this IEnumerable<T> enumerable)
        {
            Contract.Requires(enumerable != null);

            return !enumerable.GetEnumerator().MoveNext();
        }
    }
}
