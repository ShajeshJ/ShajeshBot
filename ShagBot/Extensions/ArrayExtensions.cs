using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot.Extensions
{
    public static class ArrayExtensions
    {
        public static void Shuffle<T>(this T[] array, Random rngGenerator)
        {
            for (int i = array.Count() - 1; i > 0; i--)
            {
                var swapIdx = rngGenerator.Next(i);
                var temp = array[i];
                array[i] = array[swapIdx];
                array[swapIdx] = temp;
            }
        }
    }
}
