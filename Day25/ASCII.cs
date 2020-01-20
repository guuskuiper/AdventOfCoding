using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Day5;

namespace Day25
{
    class ASCII
    {
        public enum Direction
        {
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3,
        }

        public enum Commands
        {
            north = 0,
            south = 1,
            east = 2,
            west = 3,
            take = 4,
            drop = 5,
            inv = 6,
        }

        private const int WIDTH = 53;
        private const int HEIGHT = 39;
        private bool[,] grid;
        private ElfComputer computer;

        private Direction robotDirection;
        private string inputData;
        private int inputCount;

        bool mode;

        public ASCII(IEnumerable<long> instructions)
        {
            computer = new ElfComputer(instructions, Input, Output);

            grid = new bool[WIDTH, HEIGHT];
        }

        public void Start()
        {
            computer.ProcessInstructions();
        }

        public long Input()
        {
            //var ch = inputData[inputCount];
            //System.Console.Write(ch);
            int input = Console.Read();
            inputCount++;
            return input;
            //return Convert.ToInt32(ch);
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
    }
}