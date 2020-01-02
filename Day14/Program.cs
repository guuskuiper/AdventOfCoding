using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Day14
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt");
            //var text = File.ReadAllLines("example.txt");

            var stoi = new Stoichiometry(text);
            var ore = stoi.Solve();

            System.Console.WriteLine($"ORE required: {ore}");

            var stoi2 = new Stoichiometry(text);
            var fuel = stoi2.ConsumeOre(1000000000000);

            System.Console.WriteLine($"FUEL produced: {fuel}");
        }
    }
}
