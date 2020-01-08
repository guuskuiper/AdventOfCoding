using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Day5;

namespace Day17
{
    class ASCII
    {
        private const int WIDTH = 53;
        private const int HEIGHT = 39;
        private bool[,] grid;
        private ElfComputer computer;

        private int readX = 0;
        private int readY = 0;

        public ASCII(IEnumerable<long> instructions)
        {
            computer = new ElfComputer(instructions, Input, Output);

            grid = new bool[WIDTH, HEIGHT];
        }

        public void Start()
        {
            computer.ProcessInstructions();

            FindCrossings();
        }

        public long Input()
        {
            return 0;
        }

        public void Output(long output)
        {
            var ch = Convert.ToChar(output);
            Console.Write(ch);
            switch(ch)
            {
                case '>':
                case '<':
                case 'v':
                case '^':
                case '#':
                    grid[readX, readY] = true;
                    readX++;
                    break;
                case '.':
                    grid[readX, readY] = false;
                    readX++;
                    break;
                case '\n':
                    readX = 0;
                    readY++;
                    break;
                default:
                    throw new Exception($"Unknown character {output}-{ch}");
                    break;
            }
        }

        private int FindCrossings()
        {
            // return Sum( x*y)
            int sum = 0;
            for(int y = 1; y < HEIGHT - 1; y++)
            {
                for(int x = 1; x < WIDTH - 1; x++)
                {
                    if(!grid[x,y]) continue;
                    if(!grid[x+1,y]) continue;
                    if(!grid[x-1,y]) continue;
                    if(!grid[x,y+1]) continue;
                    if(!grid[x,y-1]) continue;
                    sum += x*y;
                }
            }
            System.Console.WriteLine("Sum: " + sum);
            return sum;
        }
    }
}