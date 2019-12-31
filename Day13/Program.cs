using System;
using System.IO;
using System.Linq;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            var instructions = text.Split(',').Select(long.Parse).ToArray();
            
            var arcade = new Arcade(instructions);
            var blockCount = arcade.Start();

            System.Console.WriteLine("BlockCount " + blockCount);
        }


    }
}
