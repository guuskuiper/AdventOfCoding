using System.Diagnostics;

namespace AdventOfCode.Year2023.Day21;

[DayInfo(2023, 21)]
public class Solution21 : Solution
{
    private static readonly Point TopLeft = new Point(0, 0);
    private static readonly Point TopMiddle = new Point(65, 0);
    private static readonly Point TopRight = new Point(130, 0);
    private static readonly Point LeftMiddle = new Point(0, 65);
    private static readonly Point RightMiddle = new Point(130, 65);
    private static readonly Point BottomLeft = new Point(0, 130);
    private static readonly Point BottomMiddle = new Point(65, 130);
    private static readonly Point BottomRight = new Point(130, 130);
    
    private record State(Point P, int Step);
    
    public string Run()
    {
        string[] input = this.ReadLines();
        var grid = input.ToGrid();
        Point start = grid.Find('S').Single();

        long options = Step(grid, start, 64);
        long part2 = Step2(grid, start, 26501365);
        
        return options + "\n" + part2;
    }

    private long Step2(char[,] grid, Point start, int steps)
    {
        int tileRadius = steps / grid.Width();
        int tileRemainder = steps % grid.Width();
        
        int width = grid.Width();
        int radius = (width - 1) / 2;
        Debug.Assert(tileRemainder == radius);
        
        int cornerSteps = (steps - (radius + 1)) % width;
        int middleSteps = (steps - (radius + 1)) % width;
        int quarterSteps = cornerSteps - radius - 1;
        int threeQuarterSteps = cornerSteps - radius - 1 + width;

        Point[] middles = [LeftMiddle, RightMiddle, TopMiddle, BottomMiddle];
        Point[] quarters = [TopLeft, TopRight, BottomLeft, BottomRight];
        Point[] threeQuarters = [TopLeft, TopRight, BottomLeft, BottomRight];
        long oddFullOptions = Step(grid, start, radius * 2 - 1);
        long evenFullOptions = Step(grid, start, radius * 2);
        long middleOptions = 0;
        foreach (Point corner in middles)
        {
            middleOptions += Step(grid, corner, middleSteps);
        }
        long quortersOptions = 0;
        foreach (Point corner in quarters)
        {
            quortersOptions += Step(grid, corner, quarterSteps);
        }
        long threeQuartersOptions = 0;
        foreach (Point corner in threeQuarters)
        {
            threeQuartersOptions += Step(grid, corner, threeQuarterSteps);
        }

        long oddTileCount = 0;
        long evenTileCount = 0;
        for (int i = 0; i < tileRadius; i += 1)
        {
            bool isOdd = i % 2 == 0;
            int tileCount = OffsetCount(i);
            if (isOdd)
            {
                oddTileCount += tileCount;
            }
            else
            {
                evenTileCount += tileCount;
            }
        }        
        long total = middleOptions + 
                     tileRadius * quortersOptions + 
                     (tileRadius - 1) * threeQuartersOptions
                     + evenTileCount * evenFullOptions
                     + oddTileCount * oddFullOptions
                     ;

        long upperBound = 0;
        for (int i = 1; i <= steps; i += 2)
        {
            upperBound += OffsetCount(i);
        }

        return total;
    }

    private int OffsetCount(int radius) => radius == 0 ? 1 : 4 * radius;

    private IEnumerable<Point> Offset(Point start, int radius)
    {
        if (radius == 0) yield return start;
        for (int i = 0; i < radius; i++)
        {
            yield return new Point(start.X + radius - i, start.Y + i);
            yield return new Point(start.X - i, start.Y + radius - i);
            yield return new Point(start.X - radius + i, start.Y - i);
            yield return new Point(start.X + i, start.Y - radius + i);
        }
    }
    
    private long Step(char[,] grid, Point start, int steps, bool draw = false)
    {
        GardenGrid graph = new(grid);

        State initial = new State(start, 0);
        AQueue<State> frontier = [initial];
        HashSet<State> visited = [initial];

        while (!frontier.Empty)
        {
            State current = frontier.Get();

            if(current.Step >= steps) continue;
            
            foreach (Point next in graph.Neighbors(current.P))
            {
                State newState = new(next, current.Step + 1);
                
                if (visited.Contains(newState)) continue;
                
                frontier.Add(newState);
                visited.Add(newState);
            }
        }
        
        if(draw)
        {
            GardenStyle style = new GardenStyle(grid, visited);
            graph.Draw<Point>(style);
            long stepOptions = visited.Count(x => x.Step == steps);
            Console.WriteLine($"Options {steps}: {stepOptions}");
        }
        
        long options = visited.Count(x => x.Step == steps);
        return options;
    }
    
    private class GardenStyle : IStyle<Point>
    {
        private readonly char[,] _grid ;
        private readonly HashSet<State> _states;

        public GardenStyle(char[,] grid, HashSet<State> states)
        {
            _grid = grid;
            _states = states;
        }
        
        public int Step { get; set; }

        public string Format(Point p)
        {
            if (_states.Contains(new State(p, Step))) return "O";
            return _grid[p.X, p.Y].ToString() ?? " ";
        }
    }

    private class InfiniteGardenGrid : IRectGrid<Point>, IValueGrid<char>
    {
        private readonly char[,] _grid;
        public InfiniteGardenGrid(char[,] grid)
        {
            _grid = grid;
            Width = grid.Width();
            Height = grid.Heigth();
        }
        
        public int Width { get; }
        public int Height { get; }

        public IEnumerable<Point> Neighbors(Point node)
        {
            foreach (var neighbor in SquareGrid.SquareNeightbors)
            {
                Point result = node + neighbor;

                if (this[result] != '#')
                {
                    yield return result;
                }
            }
        }
        
        private static long Mod(long a, long length)
        {
            return (a % length + length) % length;  // always positive
        }

        public char this[Point p] => _grid[Mod(p.X, Width), Mod(p.Y, Height)]; 
    }
    private class GardenGrid : IRectGrid<Point>, IValueGrid<char>
    {
        private readonly char[,] _grid;
        public GardenGrid(char[,] grid)
        {
            _grid = grid;
            Width = grid.Width();
            Height = grid.Heigth();
        }
        
        public int Width { get; }
        public int Height { get; }

        public IEnumerable<Point> Neighbors(Point node)
        {
            foreach (var neighbor in SquareGrid.SquareNeightbors)
            {
                Point result = node + neighbor;

                if (InRange(result) && this[result] != '#')
                {
                    yield return result;
                }
            }
        }

        public char this[Point p] => _grid[p.X, p.Y];

        private bool InRange(Point p) => p.X >= 0 && p.X < Width &&
                                         p.Y >= 0 && p.Y < Height;
    }
}    