namespace AdventOfCode.Year2022.Day18;

public class Solution18 : Solution
{
    private record Point3(int X, int Y, int Z);
    
    public string Run()
    {
        string example1 = "1,1,1\r\n2,1,1";
        string example2 = """
            2,2,2
            1,2,2
            3,2,2
            2,1,2
            2,3,2
            2,2,1
            2,2,3
            2,2,4
            2,2,6
            1,2,5
            3,2,5
            2,1,5
            2,3,5 
            """;
        //var lines = example2.Split("\r\n");
        var lines = InputReader.ReadFileLinesArray();
        var points = Parse(lines);
        var grid = BuildGrid(points);
        var gridOutside = BuildOutsideGrid(grid);

        long count = Count(grid);
        long countB = CountOutside(grid, gridOutside);
        return count + "\n" + countB;
    }

    private bool[,,] BuildOutsideGrid(bool[,,] grid)
    {
        Queue<Point3> points = new();
        HashSet<Point3> visited = new();
        Point3 first = new Point3(0, 0, 0);
        points.Enqueue(first);

        while (points.Count > 0)
        {
            Point3 current = points.Dequeue();

            foreach (Point3 neighbour in Neighbours(grid, current))
            {
                if (IsInside(grid, neighbour) && !IsFilled(grid, neighbour))
                {
                    if (!visited.Contains(neighbour))
                    {
                        points.Enqueue(neighbour);
                        visited.Add(neighbour);
                    }
                }
            }
        }

        bool[,,] outside = new bool[grid.GetLength(0), grid.GetLength(1), grid.GetLength(2)];
        foreach (var p in visited)
        {
            SetGrid(outside, p, true);
        }

        return outside;
    }

    private long Count(bool[,,] grid)
    {
        long count = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    Point3 p = new Point3(x, y, z);
                    if(!IsFilled(grid, p)) continue;
                    
                    var neighbours = Neighbours(grid, p);
                    foreach (Point3 neighbour in neighbours)
                    {
                        if (!IsInside(grid, neighbour) || !IsFilled(grid, neighbour))
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
    }
    private long CountOutside(bool[,,] grid, bool[,,] outside)
    {
        long count = 0;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    Point3 p = new Point3(x, y, z);
                    if(!IsFilled(grid, p)) continue;
                    
                    var neighbours = Neighbours(grid, p);
                    foreach (Point3 neighbour in neighbours)
                    {
                        if (!IsInside(grid, neighbour))
                        {
                            count++;
                        }
                        else if (!IsFilled(grid, neighbour) && IsFilled(outside, neighbour))
                        {
                            count++;
                        }
                    }
                }
            }
        }

        return count;
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
    private IEnumerable<Point3> Neighbours(bool[,,] grid, Point3 point)
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

    private bool[,,] BuildGrid(List<Point3> point3s)
    {
        int maxX = point3s.Max(p => p.X);
        int maxY = point3s.Max(p => p.Y);
        int maxZ = point3s.Max(p => p.Z);

        bool[,,] grid = new bool[maxX + 1, maxY + 1, maxZ + 1];
        foreach (Point3 point3 in point3s)
        {
            grid[point3.X, point3.Y, point3.Z] = true;
        }

        return grid;
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
