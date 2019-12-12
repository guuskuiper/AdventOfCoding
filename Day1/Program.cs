using System;
using System.Diagnostics;
using System.IO;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = File.OpenRead("input.txt");
            var sum = 0;
            using (StreamReader reader = new StreamReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var res = ProcessLine(line);
                    var cov = Convert(res);
                    sum += cov;
                }
            }

            Console.WriteLine($"Sum = {sum}");
            Debug.Assert(sum == 3375962);

            Console.WriteLine("1Part2");
            var fuelForFuel = 0;
            file = File.OpenRead("input.txt");
            using (StreamReader reader = new StreamReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var res = ProcessLine(line);
                    var cov = RecursiveFuel(res);
                    fuelForFuel += cov;
                }
            }

            Console.WriteLine($"Sum = {fuelForFuel}");
            Debug.Assert(fuelForFuel == 5061072);
        }

        private static int ProcessLine(string line)
        {
            return int.Parse(line);
        }

        private static int Convert(int mass)
        {
            var fuel = (int)Math.Floor((double)mass / 3) - 2;
            return fuel;
        }

        private static int RecursiveFuel(int fuel)
        {
            var additionalFuel = Convert(fuel);
            if (additionalFuel >= 0)
            {
                additionalFuel += RecursiveFuel(additionalFuel);
            }
            else
            {
                return 0;
            }
            return additionalFuel;
        }
    }
}
