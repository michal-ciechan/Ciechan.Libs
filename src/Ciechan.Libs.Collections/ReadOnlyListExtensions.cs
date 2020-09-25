using System.Collections.Generic;

namespace Ciechan.Libs.Collections
{
    public static class ReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> self, T elementToFind, EqualityComparer<T>? comparer = null)
        {
            comparer ??= EqualityComparer<T>.Default;
            
            for (var i = 0; i < self.Count; i++)
            {
                T element = self[i];
                if (comparer.Equals(element, elementToFind))
                    return i;
            }

            return -1;
        }
    }
}