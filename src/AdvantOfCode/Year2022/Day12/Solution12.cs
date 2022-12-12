using System.Drawing;
using AdventOfCode.Graph;

namespace AdventOfCode.Year2022.Day12;

public class Solution12 : Solution
{
    public string Run()
    {
        var lines = InputReader.ReadFileLinesArray();

        HillGraph graph = new HillGraph(lines);

        Point start = new Point(-1, -1);
        Point end = new Point(-1, -1);

        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[0].Length; x++)
            {
                char c = lines[y][x];
                if (c == 'S') start = new Point(x, y);
                if (c == 'E') end = new Point(x, y);
            }
        }

        var parents = BFS.SearchFrom(graph, start, end);
        var path = GetPath(parents, start, end);

        var reversedParents = parents.Where(x => path.Contains(x.Key)).ToDictionary(x => x.Value, y => y.Key);
        GraphDraw.DrawGrid(graph, reversedParents, start, end);

        List<int> lengths = new();
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[0].Length; x++)
            {
                char c = lines[y][x];
                if (c != 'a') continue;
                
                start = new Point(x, y);
                Dictionary<Point, Point> resultB = BFS.SearchFrom(graph, start, end);
                List<Point> pathB = GetPath(resultB, start, end);
                int lengthPathB = pathB.Count;
                if (lengthPathB > 1)
                {
                    lengths.Add(lengthPathB);
                }
            }
        }
        lengths.Sort();
        
        return path.Count + "\n" + lengths[0];
    }
    
    private List<Point> GetPath(Dictionary<Point, Point> points, Point start, Point goal)
    {
        List<Point> path = new() { goal };
        Point current = goal;
        while (points.TryGetValue(current, out current) && current != start)
        {
            path.Add(current);
        }
        return path;
    }
    
    private class HillGraph : IRectGrid<Point>
    {
        private readonly string[] _array;
        private readonly int _width;
        private readonly int _height;
        
        private readonly Size[] _neighbors = { new (1, 0), new (-1, 0), new (0, 1), new (0, -1) };

        public HillGraph(string[] array)
        {
            _array = array;
            _height = array.Length;
            _width = array[0].Length;
        }
        
        public int Width => _width;
        public int Height => _height;

        public IEnumerable<Point> Neighbors(Point node)
        {
            foreach (var neighbor in _neighbors)
            {
                Point result = node + neighbor;

                if (InRange(result) && IsPassable(node, result))
                {
                    yield return result;
                }
            }
        }

        public bool IsPassable(Point from, Point to) => GetHeight(to) - GetHeight(from) <= 1;
        
        private bool InRange(Point p) => p.X >= 0 && p.X < _width &&
                                         p.Y >= 0 && p.Y < _height;

        private char GetHeight(Point p)
        {
            char c = _array[p.Y][p.X];
            if (c == 'S') c = 'a';
            else if (c == 'E') c = 'z';
            return c;
        }
    }
}
