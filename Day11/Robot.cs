using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Day5;

namespace Day11
{
    class Robot
    {
        private const int WIDTH = 1000;
        private const int HEIGHT = 1000;

        public enum Direction
        {
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3,
        }

        private ElfComputer computer;
        private Direction currentDirection = Direction.Up;
        private bool isColor = true;
        private int color;
        private int curX;
        private int curY;

        private int[,] panels;
        private bool[,] painted;
        private Bitmap bitmap;
        

        public Robot(IEnumerable<long> instructions)
        {
            computer = new ElfComputer(instructions, Input, Output);

            panels = new int[WIDTH, HEIGHT];
            painted = new bool[WIDTH, HEIGHT];
            bitmap = new Bitmap(WIDTH, HEIGHT);

            // start in the middle?
            curX = WIDTH / 2 + 1;
            curY = HEIGHT / 2 + 1;

            panels[curX, curY] = 1; // intialize one white panel
        }

        public int StartRobot()
        {
            computer.ProcessInstructions();

            int paintedCount = 0;
            for(int x = 0; x < WIDTH; x++)
            {
                for(int y = 0; y < HEIGHT; y++)
                {
                    if(painted[x,y])
                    {
                        paintedCount++;
                    }
                }
            }

            bitmap.Save("output.bmp");
            return paintedCount;
        }

        public long Input()
        {
            return panels[curX, curY];
            // current panel: 0 black, 1 is white
            //return 0; // or 1;
        }

        public void Output(long output)
        {
            if(isColor)
            {
                // first paint color: 0 black, 1 is white
                color = (int)output;
            }
            else
            {
                // then direction: 0 left 90, 1 right 90
                if(output == 0) TurnLeft();
                else TurnRight();
            }
            isColor = !isColor;
        }

        private void TurnLeft()
        {
            Direction newDirection;
            switch(currentDirection)
            {
                default:
                case Direction.Up: 
                    newDirection = Direction.Left;
                    break;
                case Direction.Left: 
                    newDirection = Direction.Down;
                    break;
                case Direction.Down: 
                    newDirection = Direction.Right;
                    break;
                case Direction.Right: 
                    newDirection = Direction.Up;
                    break;
            }

            currentDirection = newDirection;
            Move();
        }

        private void TurnRight()
        {
            Direction newDirection;
            switch(currentDirection)
            {
                default:
                case Direction.Up: 
                    newDirection = Direction.Right;
                    break;
                case Direction.Left: 
                    newDirection = Direction.Up;
                    break;
                case Direction.Down: 
                    newDirection = Direction.Left;
                    break;
                case Direction.Right: 
                    newDirection = Direction.Down;
                    break;
            }

            currentDirection = newDirection;
            Move();
        }

        private void Move()
        {
            Paint();

            int dx = 0;
            int dy = 0;
            switch(currentDirection)
            {
                case Direction.Up: 
                    dx = 1;
                    break;
                case Direction.Left: 
                    dy = -1;
                    break;
                case Direction.Down: 
                    dx = -1;
                    break;
                case Direction.Right: 
                    dy = 1;
                    break;
            }

            curX += dx;
            curY += dy;
        }

        private void Paint()
        {
            painted[curX, curY] = true;
            panels[curX, curY] = color;
            bitmap.SetPixel(curX, curY, color == 0 ? Color.Black : Color.White);
        }
    }
}