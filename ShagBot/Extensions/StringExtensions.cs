using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Extensions
{
    public static class StringExtensions
    {
        public static ulong ToUInt64 (this string str)
        {
            return Convert.ToUInt64(str);
        }
    }
}
