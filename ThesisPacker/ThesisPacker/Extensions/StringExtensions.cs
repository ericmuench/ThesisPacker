using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace ThesisPacker.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string? thisString, string otherString, bool ignoreCase = false)
        {
            if (ignoreCase)
            {
                return thisString?.ToLower(CultureInfo.CurrentCulture).Contains(otherString.ToLower(CultureInfo.CurrentCulture)) ?? false;
            }

            return thisString?.Contains(otherString) ?? false;
        }

        public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);
    }
}
