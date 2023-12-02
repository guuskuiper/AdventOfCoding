using System.Drawing;
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
    private readonly HashSet<Point> walls;

    private Size[] neighbors = new[] { new Size(1, 0), new Size(-1, 0), new Size(0, 1), new Size(0, -1) };

    public SquareGrid(int width, int height, IEnumerable<Point> walls)
    {
        Width = width;
        Height = height;
        this.walls = new HashSet<Point>(walls);
    }

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

    private bool InRange(Point p) => p.X >= 0 && p.X < Width &&
                                     p.Y >= 0 && p.Y < Height;

    public bool IsPassable(Point p) => !walls.Contains(p);
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
        
        return forests.TryGetValue(b, out double costs) ? costs : 1;
    }
}