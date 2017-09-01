using System.Collections;
using System.Collections.Generic;

namespace AutoDiff
{
    internal class ReadOnlyListWrapper<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> list;

        public ReadOnlyListWrapper(IReadOnlyList<T> list) { this.list = list; }
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) list).GetEnumerator();
        public int Count => list.Count;
        public T this[int index] => list[index];
    }

    internal static class ReadOnlyListWrapper
    {
        public static ReadOnlyListWrapper<T> AsReadOnly<T>(this IReadOnlyList<T> list)
        {
            return new ReadOnlyListWrapper<T>(list);
        }
    }
}