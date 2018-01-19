namespace Flee
{
    using System.Collections.Generic;

    public static class CustomMethods
    {
        public static bool IsBetween<T>(T item, T start, T end)
        {
            Comparer<T> comparer = Comparer<T>.Default;
            return comparer.Compare(item, start) >= 0 && comparer.Compare(item, end) <= 0;
        }
    }
}
