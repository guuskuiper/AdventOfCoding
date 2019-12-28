using System;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            //string text = "1,9,10,3,2,3,11,0,99,30,40,50";
            var instructions = text.Split(',').Select(int.Parse).ToArray();

            var elfComputer = new ElfComputer(instructions, Input, Output);
            elfComputer.ProcessInstructions();

            // result should be 10987514
        }

        static int Input()
        {
            return 5;
            //System.Console.WriteLine("Enter input: ");
            //return int.Parse(Console.ReadLine());
        }

        static void Output(int output)
        {
            System.Console.WriteLine("Output: " + output);
        }
    }
}
