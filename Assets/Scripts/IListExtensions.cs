using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JohnUtils
{
    public static class IListExtensions
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
        public static int[] ConvertStringToIntArray(string input)
        {
            // Split the string by commas and remove any surrounding whitespace
            string[] stringArray = input.Split(',').Select(s => s.Trim()).ToArray();

            // Convert the string array to an int array
            int[] intArray = stringArray.Select(int.Parse).ToArray();

            return intArray;
        }
    }
}
