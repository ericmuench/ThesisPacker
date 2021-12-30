using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThesisPacker.Extensions
{
    public static class CollectionExtensions
    {
        public static string PrettyPrint<T>(this IEnumerable<T> collection) => $"[{string.Join(",", collection)}]";
        public static bool IsEmpty<T>(this IEnumerable<T> collection) => collection.Any(it => true);

        public static bool ContainsDuplicates<T>(this IEnumerable<T> collection)
        {
            var data = collection.ToList();
            return data.Distinct().Count() < data.Count();
        }
    }
}
