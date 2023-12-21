namespace AdventOfCode.Year2023.Day21;

[DayInfo(2023, 21)]
public class Solution21 : Solution
{
    private record State(Point P, int Step);
    
    public string Run()
    {
        string[] input = this.ReadLines();
        var grid = input.ToGrid();
        Point start = grid.Find('S').Single();

        long options = Step(grid, start, 64);
        Step2(grid, start, 26501365);
        
        return options + "\n";
    }

    private long Step2(char[,] grid, Point start, int steps)
    {
        InfiniteGardenGrid graph = new(grid);

        for (int i = 0; i < 100; i++)
        {
            List<Point> points = Offset(start, i).ToList();
            long count = points.Count;
            long validPoints = points.Count(p => graph[p] != '#');
            Console.WriteLine($"{i}:{count} - {validPoints}");
        }
        return 0;
    }

    private IEnumerable<Point> Offset(Point start, int radius)
    {
        if (radius == 0) yield return start;
        for (int i = 0; i < radius; i++)
        {
            yield return new Point(start.X + radius - i, start.Y + i);
            yield return new Point(start.X - i, start.Y + radius - i);
            yield return new Point(start.X - radius + i, start.Y - i);
            yield return new Point(start.X + i, start.Y - radius - i);
        }
    }
    
    private long Step(char[,] grid, Point start, int steps)
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


        GardenStyle style = new GardenStyle(grid, visited);
        for (int i = 0; i < 10; i++)
        {
            break;
            style.Step = i;
            graph.Draw<Point>(style);
            long stepOptions = visited.Count(x => x.Step == i);
            Console.WriteLine($"Options {i}: {stepOptions}");
        }
        
        long options = visited.Count(x => x.Step == 64);
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