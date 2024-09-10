namespace SarcMerger.Core.Helpers;

public static class ListHelper
{
    public static bool TryGetIndex<T>(this IList<T> list, T element, IEqualityComparer<T> comparer, out int index)
    {
        int len = list.Count;
        for (int i = 0; i < len; i++) {
            if (!comparer.Equals(list[i], element)) {
                continue;
            }

            index = i;
            return true;
        }

        index = -1;
        return false;
    }
}