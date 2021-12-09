using System.Globalization;

namespace AdventOfCode.Day09;

public class Solution09 : Solution
{
    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
    
    private List<string> lines;
    private int maxX;
    private int maxY;
    private int riskLevel;
    private List<Point> lowest;
    private List<HashSet<Point>> basins;
    
    public string Run()
    {
        lines = InputReader.ReadFileLines();
        FindLowest();
        FindAllBasins();
        int B = FindLargestBasins(3);
        return riskLevel + "\n" + B;
    }

    public int FindLargestBasins(int count)
    {
        List<int> counts = basins.Select(x => x.Count).ToList();
        counts.Sort();
        var numbers = counts.TakeLast(count);

        int mult = 1;
        foreach (var number in numbers)
        {
            mult *= number;
        }

        return mult;
    }

    public void FindAllBasins()
    {
        basins = new List<HashSet<Point>>();
        foreach (var lowestPoint in lowest)
        {
            basins.Add(GetBasin(lowestPoint));
        }
    }

    public HashSet<Point> GetBasin(Point point)
    {
        HashSet<Point> basin = new HashSet<Point>();
        var lowestHeight = GetHeight(point);
        basin.Add(point);

        for (char c = lowestHeight; c < '9'; c++)
        {
            AddHigherThan(basin, c);   
        }

        return basin;
    }

    private void AddHigherThan(HashSet<Point> basin, char currentHeight)
    {
        List<Point> newPts = new List<Point>(); 
        
        foreach (var point in basin)
        {
            var ptHeight = GetHeight(point);
            if (ptHeight != currentHeight) continue;

            AddIfHigher(newPts, currentHeight, point.X + 1, point.Y);
            AddIfHigher(newPts, currentHeight, point.X - 1, point.Y);
            AddIfHigher(newPts, currentHeight, point.X, point.Y + 1);
            AddIfHigher(newPts, currentHeight, point.X, point.Y - 1);
        }

        foreach (var newPt in newPts)
        {
            basin.Add(newPt);
        }
    }

    private void AddIfHigher(List<Point> basin, char currentHeight, int x, int y)
    {
        if (x < 0 
            || x >= maxX
            || y < 0
            || y >= maxY) return;

        char height = lines[x][y]; 

        if (height != '9' && height > currentHeight)
        {
            basin.Add(new Point(x, y));
        }
    }

    public char GetHeight(Point point) => lines[point.X][point.Y];

    private void FindLowest()
    {
        lowest = new List<Point>();
        maxX = lines.Count;
        maxY = lines[0].Length;
        
        for (int i = 0; i < maxX; i++)
        {
            var line = lines[i];
            for (int j = 0; j < maxY; j++)
            {
                if (IsLowest(i, j))
                {
                    lowest.Add(new Point(x: i, y: j));
                    AddRisk(lines[i][j]);
                }
            }
        }
    }

    private void AddRisk(char c)
    {
        riskLevel += (c - '0') + 1;
    }

    private bool IsLowest(int x, int y)
    {
        char current = lines[x][y];
        bool lower = CheckLowerPoint(x - 1, y, current) &&
                     CheckLowerPoint(x + 1, y, current) &&
                     CheckLowerPoint(x, y - 1, current) &&
                     CheckLowerPoint(x, y + 1, current);

        return lower;
    }

    private bool CheckLowerPoint(int x, int y, char compared)
    {
        if (x < 0 
            || x >= maxX
            || y < 0
            || y >= maxY) return true;

        return compared < lines[x][y];
    }
}
