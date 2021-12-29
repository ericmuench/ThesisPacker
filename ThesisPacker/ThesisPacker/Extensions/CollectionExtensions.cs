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
    }
}
