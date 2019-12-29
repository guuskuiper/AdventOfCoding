using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Day5;

namespace Day9
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            //string text = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            //string text = "1102,34915192,34915192,7,4,7,99,0";
            //string text = "104,1125899906842624,99";
            var instructions = text.Split(',').Select(long.Parse).ToArray();

            var elfComputer = new ElfComputer(instructions, Input, Output);
            elfComputer.ProcessInstructions();
        }

        static long Input()
        {
            return 2;
            //System.Console.WriteLine("Enter input: ");
            //return int.Parse(Console.ReadLine());
        }

        static void Output(long output)
        {
            System.Console.WriteLine("Output: " + output);
        }
    }
}
