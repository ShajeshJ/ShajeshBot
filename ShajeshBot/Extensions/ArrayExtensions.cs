using System;
using System.Linq;

namespace ShajeshBot.Extensions
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
