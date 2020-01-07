using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day16
{
    public class FFT
    {
        private readonly int[] BASE = {0, 1, 0, -1};

        private int[] Input; 
        private int Phases;

        public FFT(IEnumerable<int> input, int phases)
        {
            Input = input.ToArray();
            Phases = phases;
            // var res = Input;
            // System.Console.WriteLine("I-" + string.Join(string.Empty, res));
            // for(int i = 0; i < phases; i++)
            // {
            //     var resSimple = PhaseSimple(res); 
            //     System.Console.WriteLine("S-" + string.Join(string.Empty, resSimple));
            //     res = Phase(res);
            //     System.Console.WriteLine("O-" + string.Join(string.Empty, res));
            //     for (int j = res.Length - 1; j >= res.Length / 2 ; j--)
            //     {
            //         if(resSimple[j] != res[j])
            //         {
            //             System.Console.WriteLine("Error from: " + j);
            //             break;
            //         }
            //     }
            // }

            // System.Console.WriteLine(string.Join(string.Empty, res.Take(8)));
        }

        public void FFTExact()
        {
            var res = Input;
            for(int i = 0; i < Phases; i++)
            {
                res = Phase(res);
            }

            System.Console.WriteLine(string.Join(string.Empty, res.Take(8)));
        }

        public void FFTSimple(int offset)
        {
            var res = Input;
            for(int i = 0; i < Phases; i++)
            {
                res = PhaseSimple(res); 
            }

            System.Console.WriteLine(string.Join(string.Empty, res.Skip(offset).Take(8)));
        }

        private int[] PhaseSimple(int[] input)
        {
            var result = new int[input.Length];
            int prevSum = 0;
            for (int i = input.Length - 1; i >= input.Length / 2 ; i--)
            {
                prevSum += input[i];
                result[i] = prevSum % 10;
            }
            return result;
        }

        private int[] Phase(int[] input)
        {
            var result = new int[input.Length];
            for (int r = 0; r < input.Length; r++)
            {
                var pattern = Pattern(input.Length, r + 1);
                var accumulate = 0;
                for (int i = 0; i < input.Length; i++)
                {
                    accumulate += input[i] * pattern[i];
                }
                result[r] = Math.Abs(accumulate) % 10;
            }

            return result;
        }

        private int[] Pattern(int length, int repeat)
        {
            return BASE.SelectMany(x => Enumerable.Repeat(x, repeat)).Repeat(1 + (int)Math.Ceiling((double)length / (repeat * BASE.Length))).Skip(1).Take(length).ToArray();
            //var repeated = BASE.SelectMany(x => Enumerable.Repeat(x, repeat)).ToList();
            // // List<int> repeated2 = new List<int> ();
            // // for(int i = 0; i < (int)Math.Ceiling((double)length / repeat); i++)
            // // {
            // //     repeated2.AddRange(repeated);
            // // }
            //var repeated2 = repeated.Repeat(1 + (int)Math.Ceiling((double)length / (repeat * BASE.Length)));
            // //var repeated2 = repeated.SelectMany(x => Enumerable.Repeat(x, 1 + (int)Math.Ceiling((double)length / repeat)));
            //return repeated2.Skip(1).Take(length).ToArray();
        }
    }
}