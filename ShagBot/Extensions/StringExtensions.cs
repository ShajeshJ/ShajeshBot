using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShagBot.Extensions
{
    public static class StringExtensions
    {
        public static ulong ToUInt64 (this string str)
        {
            return Convert.ToUInt64(str);
        }

        public static bool IsNullOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string Unescape(this string str)
        {
            return Regex.Unescape(str);
        }
    }
}
