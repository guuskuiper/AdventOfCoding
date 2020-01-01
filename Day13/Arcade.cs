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
        private int Score;
        private int Step;

        public Arcade(IEnumerable<long> instructions)
        {
            computer = new ElfComputer(instructions, Input, Output);
            computer.SetData(0, 2);

            Grid = new int[WIDTH, HEIGHT];

            commands = File.ReadAllLines("commands.txt").Select(x => int.Parse(x)).ToList();
            //File.WriteAllLines("commands.txt", commands.Select(x => x.ToString()));
        }

        public int Start()
        {
            computer.ProcessInstructions();

            System.Console.WriteLine($"Final core: {Score}");

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

        List<int> commands = new List<int>();

        public long Input()
        {
            Draw();

            var command = 0;
            if(Step < commands.Count)
            {
                command = commands[Step];
            }
            else
            {
                var input = Console.ReadKey();
 
                if(input.Key == ConsoleKey.LeftArrow) command =  -1;
                else if(input.Key == ConsoleKey.RightArrow) command = 1;
                else if(input.Key == ConsoleKey.S) 
                {
                    File.WriteAllLines("commands.txt", commands.Select(x => x.ToString()));
                    return Input();
                }
                else command = 0;

                commands.Add(command);
            }

            Step++;

            return command;
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
                    if(X == -1 && Y == 0) SetScore((int)output);
                    else SetTile((int)output);
                    break;
            }
            outputCount++;
        }

        private void SetTile(int tileId)
        {
            TileId = tileId;
            Grid[X, Y] = tileId;
        }

        private void SetScore(int score)
        {
            if(score < Score)
            {
                System.Console.WriteLine($"Highscore {Score}");
            }

            Score = score;
        }

        private void Draw()
        {
            Console.Clear();
            for(int y = 0; y < HEIGHT; y++)
            {
                Console.SetCursorPosition (0, y);
                for(int x = 0; x < WIDTH; x++)
                {
                    Console.Write(DrawTile(Grid[x,y]));
                }
            }
            Console.SetCursorPosition (0, HEIGHT);
            Console.WriteLine($"                              Score {Score}");
        }

        private char DrawTile(int tileId)
        {
            switch(tileId)
            {
                default:
                case 0:
                    return ' ';
                case 1:
                    return '|';
                case 2:
                    return '=';
                case 3:
                    return '-';
                case 4:
                    return 'O'; 
            }
        }
    }
}