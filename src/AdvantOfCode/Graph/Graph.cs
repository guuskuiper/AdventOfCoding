using System.Drawing;
using AdventOfCode.Extensions;

namespace AdventOfCode.Graph;

public class SimpleGraph : IGraph<string>
{
    private readonly Dictionary<string, List<string>> edges;


    public SimpleGraph(Dictionary<string, List<string>> edges)
    {
        this.edges = edges;
    }

    public IEnumerable<string> Neighbors(string node)
    {
        return edges[node];
    }
}

public class SquareGrid : IRectGrid<Point>
{
    public static readonly Size[] SquareNeightbors =
    {
        new(1, 0), 
        new(-1, 0), 
        new(0, 1), 
        new(0, -1)
    };
    public static readonly Size[] DiagonalNeightbors =
    {
        new(-1, -1), 
        new(0, -1), 
        new(1, -1), 
        new(1, 0), 
        new(1, 1), 
        new(0, 1), 
        new(-1, 1), 
        new(-1, 0)
    };
    
    private readonly HashSet<Point> walls;
    private readonly Size[] neighbors;

    public SquareGrid(int width, int height, IEnumerable<Point> walls, Size[] neighbors)
    {
        Width = width;
        Height = height;
        this.walls = new HashSet<Point>(walls);
        this.neighbors = neighbors;
    }
    
    public SquareGrid(int width, int height, IEnumerable<Point> walls) : this(width, height, walls, SquareNeightbors)
    { }

    public int Width { get; }
    public int Height { get; }
    
    public IEnumerable<Point> Neighbors(Point node)
    {
        foreach (var neighbor in neighbors)
        {
            Point result = node + neighbor;

            if (InRange(result) && IsPassable(result))
            {
                yield return result;
            }
        }
    }

    protected bool InRange(Point p) => p.X >= 0 && p.X < Width &&
                                     p.Y >= 0 && p.Y < Height;

    public bool IsPassable(Point p) => !walls.Contains(p);
}

public class RectValueGrid<TValue> : SquareGrid, IValueGrid<TValue>, IStyle<Point>
{
    private readonly TValue[,] _grid;
    
    public RectValueGrid(TValue[,] grid, Size[]? neightbors = null) 
        : base(grid.Width(), grid.Heigth(), Enumerable.Empty<Point>(), neightbors ?? SquareNeightbors)
    {
        _grid = grid;
    }

    public TValue this[Point point] => InRange(point) 
        ? _grid[point.X,point.Y] 
        : throw new ArgumentOutOfRangeException(nameof(point), $"Point {point} outside grid");

    public string Format(Point p) => this[p]?.ToString() ?? " ";
}

public class GridWithWeights : SquareGrid, IWeightedGraph<Point, double>
{
    private readonly Dictionary<Point, double> forests;
    
    public GridWithWeights(int width, int height, IEnumerable<Point> walls, Dictionary<Point, double> forests) : base(width, height, walls)
    {
        this.forests = forests;
    }

    public double Cost(Point a, Point b)
    {
        if (forests.Count == 0) return 1;
        
        return forests.GetValueOrDefault(b, 1);
    }
}