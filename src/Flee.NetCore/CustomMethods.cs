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

        public static bool IsIn<T>(T item, T[] targets)
        {
            Comparer<T> comparer = Comparer<T>.Default;

            for (int i = 0; i < targets.Length; i++)
            {
                T target = targets[i];
                if (comparer.Compare(item, target) == 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
