using System;
using System.IO;
using System.Linq;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            var instructions = text.Split(',').Select(long.Parse).ToArray();

            var robot = new Robot(instructions);
            robot.StartRobot();
        }
    }
}
