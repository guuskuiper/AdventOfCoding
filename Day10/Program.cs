using System;
using System.IO;
using System.Collections.Generic;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt");

            var height = text.Length;
            var width = text[0].Length;

            var map = new bool[height, width];

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    map[x,y] = text[x][y] == '#';
                }
            }

            int maxCount = 0;
            int bestX = -1;
            int bestY = -1;
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(map[x,y])
                    {
                        var count = Visible(map, x, y);
                        if(count > maxCount)
                        {
                            maxCount = count;
                            bestX = x;
                            bestY = y;
                        }
                    }
                }
            }

            System.Console.WriteLine($"Count {maxCount} ({bestX},{bestY})");
        }

        static int Visible(bool[,] map, int curX, int curY)
        {
            var height = map.GetLength(0);
            var width = map.GetLength(1);

            var angles = new HashSet<double> ();

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(map[x,y])
                    {
                        if(x == curX && y == curY) continue;
                        //var direction = (x - curX) / ( y - curY); // / 0 ??
                        var angle = Math.Atan2(y - curY, x - curX);

                        angles.Add(angle);
                    }                
                }
            }

            return angles.Count;
        }
    }
}
