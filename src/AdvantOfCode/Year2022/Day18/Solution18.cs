using AdventOfCode.Graph;

namespace AdventOfCode.Year2022.Day18;

[DayInfo(2022, 18)]
public class Solution18 : Solution
{
    private record Point3(int X, int Y, int Z);
    
    public string Run()
    {
        string[] lines = this.ReadLines();
        var points = Parse(lines);
        var grid = BuildGrid(points);
        long count = Count(grid);
        
        var gridOutside = BuildOutsideGrid(grid);
        var externalGrid = ExternalGrid(grid, gridOutside);
        long countB = Count(externalGrid);
        
        return count + "\n" + countB;
    }

    private long Count(bool[,,] grid)
    {
        long count = 0;
        foreach (var p in GridPoints(grid))
        {
            if(!IsFilled(grid, p)) continue;
            
            var neighbours = Neighbours(p);
            foreach (Point3 neighbour in neighbours)
            {
                if (!IsInside(grid, neighbour) || !IsFilled(grid, neighbour))
                {
                    count++;
                }
            }
        }

        return count;
    }
    
    private IEnumerable<Point3> GridPoints(bool[,,] grid)
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    Point3 p = new Point3(x, y, z);
                    yield return p;
                }
            }
        }
    }

    private bool IsInside(bool[,,] grid, Point3 point)
    {
        return point.X >= 0 && point.X < grid.GetLength(0) &&
            point.Y >= 0 && point.Y < grid.GetLength(1) &&
            point.Z >= 0 && point.Z < grid.GetLength(2);
    }
    
    private bool IsFilled(bool[,,] grid, Point3 point)
    {
        return grid[point.X, point.Y, point.Z];
    }
    
    private void SetGrid(bool[,,] grid, Point3 point, bool value)
    {
        grid[point.X, point.Y, point.Z] = value;
    }

    private static readonly int[] Offsets = { -1, 1 };
    private IEnumerable<Point3> Neighbours(Point3 point)
    {
        foreach (var dx in Offsets)
        {
            int x = point.X + dx;
            yield return point with { X = x };
        }

        foreach (var dy in Offsets)
        {
            int y = point.Y + dy;
            yield return point with { Y = y };
        }

        foreach (var dz in Offsets)
        {
            int z = point.Z + dz;
            yield return point with { Z = z };
        }
    }

    private bool[,,] BuildGrid(List<Point3> points)
    {
        int maxX = points.Max(p => p.X);
        int maxY = points.Max(p => p.Y);
        int maxZ = points.Max(p => p.Z);

        bool[,,] grid = new bool[maxX + 1, maxY + 1, maxZ + 1];
        foreach (Point3 point3 in points)
        {
            grid[point3.X, point3.Y, point3.Z] = true;
        }

        return grid;
    }

    private class CubeGraph : IGraph<Point3>
    {
        private readonly bool[,,] _grid;
        
        public CubeGraph(bool[,,] grid)
        {
            _grid = grid;
        }
        
        public IEnumerable<Point3> Neighbors(Point3 node)
        {
            foreach (Point3 neighbor in NeighborsCore(node))
            {
                if (IsInsideBounds(neighbor) && !IsCube(neighbor))
                {
                    yield return neighbor;
                }
            }
        }
        
        private static readonly int[] Offsets = { -1, 1 };
        private IEnumerable<Point3> NeighborsCore(Point3 point)
        {
            foreach (var dx in Offsets)
            {
                int x = point.X + dx;
                yield return point with { X = x };
            }

            foreach (var dy in Offsets)
            {
                int y = point.Y + dy;
                yield return point with { Y = y };
            }

            foreach (var dz in Offsets)
            {
                int z = point.Z + dz;
                yield return point with { Z = z };
            }
        }
        
        private bool IsInsideBounds(Point3 point)
        {
            return point.X >= 0 && point.X < _grid.GetLength(0) &&
                   point.Y >= 0 && point.Y < _grid.GetLength(1) &&
                   point.Z >= 0 && point.Z < _grid.GetLength(2);
        }
        private bool IsCube(Point3 point)
        {
            return _grid[point.X, point.Y, point.Z];
        }
    }

    private bool[,,] BuildOutsideGrid(bool[,,] grid)
    {
        CubeGraph graph = new CubeGraph(grid);
        var parents = BFS.SearchFrom(graph, new Point3(0, 0, 0), null);
        
        bool[,,] outside = new bool[grid.GetLength(0), grid.GetLength(1), grid.GetLength(2)];
        foreach (var p in parents.Keys)
        {
            SetGrid(outside, p, true);
        }

        return outside;
    }
    
    private bool[,,] ExternalGrid(bool[,,] grid, bool[,,] outside)
    {
        bool[,,] result = new bool[grid.GetLength(0), grid.GetLength(1), grid.GetLength(2)];

        for (int x = 0; x < result.GetLength(0); x++)
        {
            for (int y = 0; y < result.GetLength(1); y++)
            {
                for (int z = 0; z < result.GetLength(2); z++)
                {
                    bool isCube = grid[x, y, z];
                    bool isOutside = outside[x, y, z];
                    result[x, y, z] = isCube || !isOutside;
                }
            } 
        }

        return result;
    }

    private List<Point3> Parse(IEnumerable<string> lines)
    {
        List<Point3> points = new();
        foreach (var line in lines)
        {
            string[] split = line.Split(',');
            int[] splitInt = split.Select(x => int.Parse(x)).ToArray();
            Point3 point = new Point3(splitInt[0], splitInt[1], splitInt[2]);
            points.Add(point);
        }

        return points;
    }
}
