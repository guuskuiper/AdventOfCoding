using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllLines("input.txt");
            //var text = File.ReadAllLines("example.txt");

            var height = text.Length;
            var width = text[0].Length;

            var map = new bool[width, height];

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    map[x,y] = text[y][x] == '#';
                }
            }

            int maxCount = 0;
            int bestX = -1;
            int bestY = -1;
            var mapCount = new int[width, height];
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(map[x,y])
                    {
                        var count = Visible(map, x, y);
                        mapCount[x,y] = count;
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

            // Lasers!
            var part2 = ShootLasers(map, bestX, bestY);
            //var part2 = ShootLasers(map, 8, 3);

            System.Console.WriteLine($"Part2: {part2}");
        }

        static int Visible(bool[,] map, int curX, int curY)
        {
            var height = map.GetLength(1);
            var width = map.GetLength(0);

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

        static int ShootLasers(bool[,] map, int curX, int curY)
        {
            var height = map.GetLength(1);
            var width = map.GetLength(0);

            // create a ordered data structure of planets per angle, the planets are sorted by distance
            var data = new SortedList<double, SortedList<double, (int, int)>>();

            // add all planets
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    if(map[x,y])
                    {
                        if(x == curX && y == curY) continue;
                        var dy = y - curY;
                        var dx = x - curX;
                        var angle = Math.Atan2(-dy, dx);
                        if(angle < 0)
                        {
                            angle = 2 * Math.PI + angle; // -> 0 .. 2PI
                        }
                        angle = (angle - Math.PI / 2 + Math.PI * 2) % (Math.PI * 2); // start at (0, 1)
                        angle = -angle; // reverse direction
                        angle = (angle + Math.PI * 2) % (Math.PI * 2); // -> 0 .. 2PI

                        var distance = Math.Sqrt(dy * dy + dx * dx);

                        if(!data.ContainsKey(angle))
                        {
                            data.Add(angle, new SortedList<double, (int, int)>());
                        }
                        data[angle].Add(distance, (x, y));
                    }                
                }
            }

            int removedAstroids = 0;

            while(true)
            {
                foreach(KeyValuePair<double, SortedList<double, (int, int)>> kvp in data)
                {
                    //if(first && kvp.Key < startAngle) continue;

                    if(kvp.Value.Count > 0)
                    {
                        var currentList = kvp.Value;
                        var currentAstroid = currentList.Values[0];
                        kvp.Value.RemoveAt(0);
                        removedAstroids++;

                        if(removedAstroids == 200)
                        {
                             return (currentAstroid.Item1 * 100 + currentAstroid.Item2);
                        }
                    }
                }
            }
        }
    }
}
