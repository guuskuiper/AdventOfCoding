using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day10;

[DayInfo(2019, 10)]
public class Solution10 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        
        var height = input.Length;
        var width = input[0].Length;

        var map = new bool[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                map[x,y] = input[y][x] == '#';
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

        // Lasers!
        var part2 = ShootLasers(map, bestX, bestY);

        return maxCount + "\n" + part2;
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