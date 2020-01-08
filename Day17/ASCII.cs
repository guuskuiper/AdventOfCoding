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
        public enum Direction
        {
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3,
        }

        private const int WIDTH = 53;
        private const int HEIGHT = 39;
        private bool[,] grid;
        private ElfComputer computer;

        private int readX = 0;
        private int readY = 0;

        private int robotX;
        private int robotY;
        private Direction robotDirection;

        public ASCII(IEnumerable<long> instructions)
        {
            computer = new ElfComputer(instructions, Input, Output);

            grid = new bool[WIDTH, HEIGHT];
        }

        public void Start()
        {
            computer.ProcessInstructions();

            FindCrossings();
            StraightPath();
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
                    robotX = readX;
                    robotY = readY;
                    robotDirection = Direction.Right;
                    grid[readX, readY] = true;
                    readX++;
                    break;
                case '<':
                    robotX = readX;
                    robotY = readY;
                    robotDirection = Direction.Left;
                    grid[readX, readY] = true;
                    readX++;
                    break;
                case 'v':
                    robotX = readX;
                    robotY = readY;
                    robotDirection = Direction.Down;
                    grid[readX, readY] = true;
                    readX++;
                    break;
                case '^':
                    robotX = readX;
                    robotY = readY;
                    robotDirection = Direction.Up;
                    grid[readX, readY] = true;
                    readX++;
                    break;
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
            int crossings = 0;
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
                    crossings++;
                    System.Console.WriteLine($"Crossing at {x},{y}");
                }
            }
            System.Console.WriteLine($"Crossings {crossings} Sum: {sum}");
            return sum;
        }

        private void StraightPath()
        {
            int curX = robotX;
            int curY = robotY;
            Direction curDir = robotDirection;

            int tempX = 0;
            int tempY = 0;

            int steps = 0;
            string commands = "";
            while(true)
            {
                if(DirectionPosition(curX, curY, curDir, out tempX, out tempY))
                {
                    steps++;
                    curX = tempX;
                    curY = tempY;
                }
                else
                {
                    // find new direction
                    commands += steps;
                    steps = 0;
                    bool turn = false; // false = left, true = right
                    if(DirectionPosition(curX, curY, Direction.Up, out tempX, out tempY))
                    {
                        turn = curDir == Direction.Left;
                        curDir = Direction.Up;
                    }
                    else if(DirectionPosition(curX, curY, Direction.Left, out tempX, out tempY))
                    {
                        turn = curDir == Direction.Down;
                        curDir = Direction.Left;
                    }
                    else if(DirectionPosition(curX, curY, Direction.Down, out tempX, out tempY))
                    {
                        turn = curDir == Direction.Right;
                        curDir = Direction.Down;
                    }
                    else if(DirectionPosition(curX, curY, Direction.Right, out tempX, out tempY))
                    {
                        turn = curDir == Direction.Up;
                        curDir = Direction.Right;
                    }
                    else
                    {
                        throw new Exception("Crashed!!!");
                    }
                    commands.Append(turn ? 'R' : 'L');
                }
                System.Console.WriteLine(commands);
            }
        }

        private bool DirectionPosition(int x, int y, Direction directory, out int x2, out int y2)
        {
            switch(directory)
            {
                case Direction.Up:
                    x2 = x - 1;
                    y2 = y;
                    if(x2 < 0) return false;
                    break;
                case Direction.Down:
                    x2 = x + 1;
                    y2 = y;
                    if(x2 >= WIDTH) return false;
                    break;
                case Direction.Left:
                    x2 = x;
                    y2 = y - 1;
                    if(y2 < 0) return false;
                    break;
                case Direction.Right:
                    x2 = x;
                    y2 = y + 1;
                    if(y2 >= HEIGHT) return false;
                    break;
                default:
                    x2 = -1;
                    y2 = -1;
                    return false;
            }

            if(!grid[x2, y2]) return false;

            return true;
        }
    }
}