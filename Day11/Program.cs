using System;
using System.IO;
using System.Linq;
using Day5;

namespace Day11
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            var instructions = text.Split(',').Select(long.Parse).ToArray();

            var robot = new Robot(instructions);
            var painted = robot.StartRobot();

            System.Console.WriteLine("Painted " + painted);
        }
    }
}
