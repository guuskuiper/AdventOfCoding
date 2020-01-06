using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day16
{
    public class FFT
    {
        private readonly int[] BASE = {0, 1, 0, -1}; 

        public FFT(IEnumerable<int> input, int phases)
        {
            var res = input.ToArray();
            for(int i = 0; i < phases; i++)
            {
                res = Phase(res);
            }

            System.Console.WriteLine(string.Join(' ', res.Take(8)));
        }

        private int[] Phase(int[] input)
        {
            var result = new int[input.Length];
            for (int r = 0; r < input.Length; r++)
            {
                var pattern = Pattern(input.Length, r + 1);
                for (int i = 0; i < input.Length; i++)
                {
                    result[r] += input[i] * pattern[i];
                }
                result[r] = Math.Abs(result[r]) % 10;
            }

            return result;
        }

        private int[] Pattern(int length, int repeat)
        {
            var repeated = BASE.SelectMany(x => Enumerable.Repeat(x, repeat)).ToList();
            List<int> repeated2 = new List<int> ();
            for(int i = 0; i < (int)Math.Ceiling((double)length / repeat); i++)
            {
                repeated2.AddRange(repeated);
            }
            //var repeated2 = repeated.SelectMany(x => Enumerable.Repeat(x, 1 + (int)Math.Ceiling((double)length / repeat)));
            return repeated2.Skip(1).Take(length).ToArray();
        }
    }
}