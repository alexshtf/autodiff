namespace AutoDiff.Tests
{
    static class Utils
    {
        public static T[] Array<T>(params T[] items)
        {
            return items;
        }

        public static double[] Vector(params double[] items)
        {
            return Array(items);
        }
    }
}
