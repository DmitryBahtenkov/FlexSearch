using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlexSearch.Panel.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable is null || !enumerable.Any();
        }
    }
}