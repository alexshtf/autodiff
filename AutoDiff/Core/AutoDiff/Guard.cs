using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoDiff
{
    internal static class Guard
    {
        public static void CollectionAndItemsNotNull<T>(IEnumerable<T> arg, string argName) where T : class
        {
            NotNull(arg, argName);
            ItemsNotNull(arg, argName);
        }

        public static void NotNull<T>(T arg, string argName) where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }
        
        public static void NotNullOrEmpty<T>(IEnumerable<T> arg, string argName)
        {
            NotNull(arg, argName);
            if (!arg.Any())
                throw new ArgumentException(string.Format("{0} must not be an empty enumerable", argName));
        }

        public static void ItemsNotNull<T>(IEnumerable<T> arg, string argName) where T : class
        {
            foreach (T item in arg)
                if (item == null)
                    throw new ArgumentException(string.Format("All items in {0} must not be null", argName));
        }

        public static void InRange(int i, string argName, int lbInclusive = int.MinValue, int ubExclusive = int.MaxValue)
        {
            if (i < lbInclusive || i >= ubExclusive)
            {
                var msg = string.Format("must be at least {0} and below {1}", lbInclusive, ubExclusive);
                throw new ArgumentOutOfRangeException(argName, i, msg);
            }
        }

        public static void MustHold(bool flag, string msg)
        {
            if (!flag)
                throw new ArgumentException(msg);
        }
    }
}
