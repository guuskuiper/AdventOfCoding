using System;
using System.IO;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt");

            var orbits = new Orbits(text);
            var result = orbits.GetOrbitCount();
            System.Console.WriteLine(result);
        }
    }
}
