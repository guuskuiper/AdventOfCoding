using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Day5;

namespace Day13
{
    class Arcade
    {
        private const int WIDTH = 44;
        private const int HEIGHT = 22;

        private ElfComputer computer;
        private int[,] Grid;
        private int outputCount;
        private int X;
        private int Y;
        private int TileId;

        public Arcade(IEnumerable<long> instructions)
        {
            computer = new ElfComputer(instructions, Input, Output);

            Grid = new int[WIDTH, HEIGHT];
        }

        public int Start()
        {
            computer.ProcessInstructions();

            return CountBlockTiles();
        }

        private int CountBlockTiles()
        {
            int blockCount = 0;
            for(int x = 0; x < Grid.GetLength(0); x++)
            {
                for(int y = 0; y < Grid.GetLength(1); y++)
                {
                    if(Grid[x,y] == 2) blockCount ++;
                }
            }
            return blockCount;
        }

        public long Input()
        {
            return 0;
        }

        public void Output(long output)
        {
            switch(outputCount % 3)
            {
                case 0:
                    X = (int)output;
                    break;
                case 1:
                    Y = (int)output;
                    break;
                case 2:
                    TileId = (int)output;
                    Grid[X, Y] = TileId;
                    break;
            }
            outputCount++;
        }
    }
}