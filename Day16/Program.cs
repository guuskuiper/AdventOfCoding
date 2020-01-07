using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Day16
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt")[0];
            //var text = "03036732577212944063491565474664";
            //var text = "80871224585914546619083218645595";

            int[] digits = text.Select(char.GetNumericValue).Select(Convert.ToInt32).ToArray();

            var fft = new FFT(digits, 100);
            fft.FFTExact();

            var digits2 = digits.Repeat(10000).ToArray();
            var fft2 = new FFT(digits2, 100);
            var offset = int.Parse(text.Substring(0, 7));
            fft2.FFTSimple(offset);
        }
    }
}
