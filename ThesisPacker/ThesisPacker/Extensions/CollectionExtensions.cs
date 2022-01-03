using System.Collections.Generic;
using System.Linq;

namespace ThesisPacker.Extensions
{
    public static class CollectionExtensions
    {
        public static string PrettyPrint<T>(this IEnumerable<T> collection) => $"[{string.Join(",", collection)}]";
        public static bool IsEmpty<T>(this IEnumerable<T> collection) => collection.ToList().Count == 0;

        public static bool ContainsDuplicates<T>(this IEnumerable<T> collection)
        {
            var data = collection.ToList();
            return data.Distinct().Count() < data.Count();
        }
    }
}
