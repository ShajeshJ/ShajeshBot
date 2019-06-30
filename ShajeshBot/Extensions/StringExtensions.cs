using System;
using System.Text.RegularExpressions;

namespace ShajeshBot.Extensions
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

        public static int TryParseInt(this string str, int fallback = 0)
        {
            if (int.TryParse(str, out var value))
                return value;
            else
                return fallback;
        }
    }
}
