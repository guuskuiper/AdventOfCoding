using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Day5;

namespace Day25
{
    class ASCII
    {
        private const char NEWLINE = '\n';
        private ElfComputer computer;

        private string inputData;
        private int inputCount;

        public ASCII(IEnumerable<long> instructions)
        {
            computer = new ElfComputer(instructions, Input, Output);
        }

        public void Start()
        {
            computer.ProcessInstructions();
        }

        public void Start(string commands)
        {
            inputCount = 0;
            var input = commands.Replace(Environment.NewLine, NEWLINE.ToString());

            var options = string.Join("west\n", GenerateOptions());

            inputData = input + options;
            Start();
        }

        public long Input()
        {
            int input;
            if(inputCount < inputData.Length)
            {
                input = Convert.ToInt32(inputData[inputCount]);
                System.Console.Write(Convert.ToChar(input));
            }
            else
            {
                input = Console.Read();
            }
            inputCount++;
            return input;
        }

        public void Output(long output)
        {
            if(output > 0x7F)
            {
                System.Console.WriteLine("Output: " + output);
                return;
            }
            var ch = Convert.ToChar(output);

            Console.Write(ch);
        }

        private IEnumerable<string> GenerateOptions()
        {
            string[] items =  
            {
                "jam", 
                "bowl of rice",
                "antenna",
                "manifold",
                "hypercube",
                "dehydrated water",
                "candy cane",
                "dark matter",
            };
            var options = Enumerable.Range(0, (int)Math.Pow(2, items.Length));
            foreach(var option in options)            
            {
                var sb = new StringBuilder();
                var bits = new BitArray(BitConverter.GetBytes(option));
                for(int i = 0; i < items.Length; i++)
                {
                    
                    sb.AppendLine( (bits[i] ? "take" : "drop") + " " + items[i]);
                }
                yield return sb.ToString().Replace(Environment.NewLine, NEWLINE.ToString());
            }
        }
    }
}