using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllText("input.txt");
            //string text = "1,9,10,3,2,3,11,0,99,30,40,50";
            var instructions = text.Split(',').Select(int.Parse).ToArray();

            //// change input
            instructions[1] = 12;
            instructions[2] = 2;

            var elfComputer = new ElfComputer(instructions);
            elfComputer.ProcessInstructions();

            Debug.Assert(elfComputer.GetData(0) == 12490719);

            for (int noun = 0; noun <= 99; noun++)
            {
                for (int verb = 0; verb <= 99; verb++)
                {
                    instructions[1] = noun;
                    instructions[2] = verb;

                    elfComputer = new ElfComputer(instructions);
                    elfComputer.ProcessInstructions();

                    if (elfComputer.GetData(0) == 19690720)
                    {
                        Console.WriteLine($"Noun {noun}, verb {verb}");
                        return;
                    }
                }
            }

            Console.WriteLine("No solution found");
        }
    }
}
