using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Day5;

namespace Day21
{
    class SpringDroid
    {
        private ElfComputer computer;
        private string inputData;
        private int inputCount;

        public SpringDroid(IEnumerable<long> input)
        {
            computer = new ElfComputer(input, Input, Output);
        }

        private long Input()
        {


            var ch = inputData[inputCount];
            System.Console.Write(ch);
            inputCount++;
            return Convert.ToInt32(ch);
        }

        private void Output(long output)
        {
            if(output > 0x7F)
            {
                System.Console.WriteLine("Output: " + output);
                return;
            }
            var ch = Convert.ToChar(output);

            Console.Write(ch);
        }

        public void Start(string inputs)
        {
            inputData = inputs;
            inputCount = 0;

            computer.ProcessInstructions();
        }
    }
}