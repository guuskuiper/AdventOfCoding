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
            //var text = "80871224585914546619083218645595";

            int[] digits = text.Select(char.GetNumericValue).Select(Convert.ToInt32).ToArray();

            var fft = new FFT(digits, 100);
        }
    }
}
