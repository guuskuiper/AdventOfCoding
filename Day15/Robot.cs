using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Day5;

namespace Day15
{
    class Robot
    {
        private const int WIDTH = 65;
        private const int HEIGHT = 65;

        public enum Movement
        {
            North = 1, 
            South = 2, 
            West = 3, 
            East = 4
        }

        public enum Location
        {
            Unknown = 0,
            Wall = 1,
            Target = 2,
            Empty = 3,
        }

        private ElfComputer computer;
        private int curX;
        private int curY;
        private int targetX;
        private int targetY;

        readonly int startX;
        readonly int startY;

        private Location[,] grid;
        private Bitmap bitmap;
        private Movement currentMove;
        private bool foundWall;
        

        public Robot(IEnumerable<long> instructions)
        {
            computer = new ElfComputer(instructions, Input, Output);

            grid = new Location[WIDTH, HEIGHT];
            bitmap = new Bitmap(WIDTH, HEIGHT);

            // start in the middle?
            startX = WIDTH / 2 + 1;
            startY = HEIGHT / 2 + 1;
            curX = startX;
            curY = startY;
            currentMove = Movement.North;

            targetX = startX;
            targetY = startY;
        }

        public int StartRobot()
        {
            computer.ProcessInstructions();

            return 0;
        }

        public long Input()
        {
            var previousMove = currentMove;
            // previous hit a wall?

            bool searchWall = false;

            if(!foundWall)
            {
                currentMove = previousMove;
                UpdateTarget(currentMove);
            }
            else if(grid[targetX, targetY] != Location.Wall)
            {
                // no wall found from the previous target
                // look for a wall by turning left
                searchWall = true;
                currentMove = TurnLeft(previousMove);
                UpdateTarget(currentMove);

                if(grid[targetX, targetY] == Location.Wall)
                {
                    // dont turn towards a wall
                    currentMove = previousMove;
                    UpdateTarget(currentMove);
                }
            }
            else
            {
                // avoid wall by turning right
                currentMove = TurnRight(previousMove);
                UpdateTarget(currentMove);
            }

            //if(searchWall && grid[targetX, targetY] != Location.Unknown)
            if(targetX == startX && targetY == startY)
            {
                Display();
                throw new Exception("Done");
            }

            return (long)currentMove;
        }

        private Movement TurnRight(Movement move)
        {
            switch(move)
            {
                case Movement.North:
                    return Movement.East;
                case Movement.East:
                    return Movement.South;
                case Movement.South:
                    return Movement.West;
                case Movement.West:
                    return Movement.North;
                default:
                    throw new Exception("Unknow movement");
            }
        }

        private Movement TurnLeft(Movement move)
        {
            switch(move)
            {
                case Movement.North:
                    return Movement.West;
                case Movement.East:
                    return Movement.North;
                case Movement.South:
                    return Movement.East;
                case Movement.West:
                    return Movement.South;
                default:
                    throw new Exception("Unknow movement");
            }
        }

        private void UpdateTarget(Movement move)
        {
            switch(move)
            {
                case Movement.North:
                    targetX = curX;
                    targetY = curY+1;
                    break;
                case Movement.East:
                    targetX = curX+1;
                    targetY = curY;
                    break;
                case Movement.South:
                    targetX = curX;
                    targetY = curY-1;
                    break;
                case Movement.West:
                    targetX = curX-1;
                    targetY = curY;
                    break;
                default:
                    throw new Exception("Unknow movement");
            }
        }

        public void Output(long output)
        {
            switch(output)
            {
                case 0:
                    grid[targetX, targetY] = Location.Wall;
                    System.Console.WriteLine($"Wall at ({targetX},{targetY})");
                    foundWall = true;
                    break;
                case 1:
                    grid[targetX, targetY] = Location.Empty;
                    break;
                case 2:
                    grid[targetX, targetY] = Location.Target;
                    break;
                default:
                    throw new Exception("Unknow output");
            }

            if(output != 0)
            {
                curX = targetX;
                curY = targetY;
            }
        }

        private void Display()
        {
            Console.Clear();
            for(int y = 0; y < HEIGHT; y++)
            {
                Console.SetCursorPosition (0, y);
                for(int x = 0; x < WIDTH; x++)
                {
                    Console.Write(Draw(grid[x,y]));
                }
            }

            Console.SetCursorPosition(startX, startY);
            Console.Write('S');
            Console.SetCursorPosition(0, HEIGHT);
        }

        private char Draw(Location location)
        {
            switch(location)
            {
                case Location.Empty:
                    return ' ';
                case Location.Target:
                    return '2';
                case Location.Unknown:
                    return '?';
                case Location.Wall:
                    return '#';
                default:
                    throw new Exception();
            }
        }
    }
}