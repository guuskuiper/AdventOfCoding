using System;
using System.Linq;
using System.Collections.Generic;

namespace Day16
{
    public static class EnumerableExtentions
    {
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, int count)
        {
            var sourceList = source.ToList();
            for(int i = 0; i < count; i++)
            {
                for(int j = 0; j < sourceList.Count; j++)
                {
                    yield return sourceList[j];
                }
            }
        }
    }
}