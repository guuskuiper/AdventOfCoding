using System.Diagnostics;

namespace AdventOfCode.Year2023.Day22;

[DayInfo(2023, 22)]
public class Solution22 : Solution
{
    private class Node
    {
        private readonly int _id;
        public Node(int id, Brick brick)
        {
            _id = id;
            Brick = brick;
        }
        
        public Brick Brick { get; }
        public int Id => _id;
        
        public List<Node> Children = [];
        public List<Node> Parents = [];
    }
    private record struct Point3(int X, int Y, int Z);

    private record Brick(Point3 Start, Point3 End)
    {
        public int Size()
        {
            int x = End.X - Start.X + 1;
            int y = End.Y - Start.Y + 1;
            int z = End.Z - Start.Z + 1;
            return x * y * z;
        }
        public IEnumerable<Point3> AllPoints()
        {
            for (int x = Start.X; x <= End.X; x++)
            {
                for (int y = Start.Y; y <= End.Y; y++)
                {
                    for (int z = Start.Z; z <= End.Z; z++)
                    {
                        yield return new Point3(x, y, z);
                    }
                }
            }
        }

        public IEnumerable<Point> XYPoints()
        {
            for (int x = Start.X; x <= End.X; x++)
            {
                for (int y = Start.Y; y <= End.Y; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }
    
    public string Run()
    {
        string[] input = this.ReadLines();

        Brick[] bricks = input.Select(ParseLine).ToArray();

        (long sum, long chainSum) = Part1(bricks);
        
        return sum + "\n" + chainSum;
    }

    private (long sum, long chainSum) Part1(Brick[] bricks)
    {
        Dictionary<int, Brick> brickMap = [];
        for (int i = 0; i < bricks.Length; i++)
        {
            Brick b = bricks[i];
            brickMap[i + 1] = b;
        }
        
        int[,,] space = Create3DGrid(brickMap);

        int countBefore = CountFilled(space);
        ApplyFall(space, brickMap);
        int countAfter = CountFilled(space);
        int cells = bricks.Select(x => x.Size()).Sum();
        
        Debug.Assert(countBefore == countAfter);
        Debug.Assert(cells == countAfter);

        Dictionary<int, HashSet<int>> supportingMap = [];
        for (int z = 1; z < space.GetLength(2); z++)
        {
            HashSet<int> bricksOnLayer = [];
            for (int x = 0; x < space.GetLength(0); x++)
            {
                for (int y = 0; y < space.GetLength(1); y++)
                {
                    int brickKey = space[x, y, z];
                    if (brickKey > 0)
                    {
                        bricksOnLayer.Add(brickKey);
                    }
                }
            }

            foreach (int brickId in bricksOnLayer)
            {
                Brick brick = brickMap[brickId];
                
                // only look for supports on the lowest z value
                if(brick.Start.Z != z) continue; 
                
                HashSet<int> supportedBy = SupportedBy(space, brick);
                foreach (var support in supportedBy)
                {
                    if (supportingMap.TryGetValue(support, out HashSet<int>? values))
                    {
                        values.Add(brickId);
                    }
                    else
                    {
                        supportingMap[support] = [brickId];
                    }
                }
            }
        }
        
        Dictionary<int, Node> nodes = CreateTree(supportingMap, brickMap, Enumerable.Range(1, bricks.Length).ToList());

        // Part 1
        int sum = 0;
        foreach (var brickId in Enumerable.Range(1, bricks.Length))
        {
            Node node = nodes[brickId];
            bool canBeDisintegrate = true;
            foreach (Node child in node.Children)
            {
                if (child.Parents.Count == 1)
                {
                    canBeDisintegrate = false;
                }
            }
            sum += canBeDisintegrate ? 1 : 0;
        }

        int chainSum = 0;
        foreach (var brickId in Enumerable.Range(1, bricks.Length))
        {
            Node brickNode = nodes[brickId];
            
            // order nodes by increasing End.Z
            PriorityQueue<Node, int> queue = new();
            queue.Enqueue(brickNode, NodeCosts(brickNode));
            
            HashSet<Node> visited = [brickNode];
            HashSet<Node> collapse = [brickNode];

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();

                foreach (Node child in current.Children)
                {
                    if (visited.Add(child))
                    {
                        if (child.Parents.All(p => collapse.Contains(p)))
                        {
                            collapse.Add(child);
                            queue.Enqueue(child, NodeCosts(child));
                        }
                    }
                }
            }

            chainSum += collapse.Count - 1; // minus the starting node
        }
        
        return (sum, chainSum);
    }

    private int NodeCosts(Node n) => n.Brick.End.Z;

    private static void ApplyFall(int[,,] space, Dictionary<int, Brick> brickMap)
    {
        HashSet<int> fixedBricks = [];
        for (int z = 1; z < space.GetLength(2); z++)
        {
            HashSet<int> bricksOnLayer = [];
            for (int x = 0; x < space.GetLength(0); x++)
            {
                for (int y = 0; y <space.GetLength(1); y++)
                {
                    int brickKey = space[x, y, z];
                    if (brickKey > 0 && !fixedBricks.Contains(brickKey))
                    {
                        bricksOnLayer.Add(brickKey);
                    }
                }
            }

            foreach (int brickId in bricksOnLayer)
            {
                while (true)
                {
                    Brick brick = brickMap[brickId];
                    bool isSupported = IsSupported(space, brick, brick.Start.Z);
                    if(isSupported) break;
                    
                    // move down, possible multiple Z's
                    Brick moved = new Brick(
                        brick.Start with { Z = brick.Start.Z - 1 }, 
                        brick.End with { Z = brick.End.Z - 1 }
                    );
                    
                    SetGrid(space, brick, 0);
                    SetGrid(space, moved, brickId);

                    brickMap[brickId] = moved;
                }
                fixedBricks.Add(brickId);
            }
        }
    }

    private static int[,,] Create3DGrid(Dictionary<int, Brick> brickMap)
    {
        int maxX = brickMap.Values.Select(b => Math.Max(b.Start.X, b.End.X)).Max();
        int maxY = brickMap.Values.Select(b => Math.Max(b.Start.Y, b.End.Y)).Max();
        int maxZ = brickMap.Values.Select(b => Math.Max(b.Start.Z, b.End.Z)).Max();

        int[,,] space = new int[maxX + 1, maxY + 1, maxZ + 1];

        foreach ((int key, Brick brick) in brickMap)
        {
            SetGrid(space, brick, key);
        }

        return space;
    }

    private Dictionary<int, Node> CreateTree(
        Dictionary<int, HashSet<int>> supportingMap,
        Dictionary<int, Brick> brickMap,
        List<int> brickIds)
    {
        Dictionary<int, Node> nodes = [];
        foreach (var brickId in brickIds)
        {
            Brick brick = brickMap[brickId];
            Node node = new Node(brickId, brick);
            nodes[brickId] = node;
        }

        foreach ((var brickId, HashSet<int> value) in supportingMap)
        {
            Node parent = nodes[brickId];
            foreach (var childId in value)
            {
                Node child = nodes[childId];
                child.Parents.Add(parent);
                parent.Children.Add(child);
            }
        }
        return nodes;
    }

    private int CountFilled(int[,,] grid)
    {
        int count = 0;
        foreach (int cell in grid)
        {
            if (cell > 0) count++;
        }
        return count;
    }

    private static bool IsSupported(int[,,] space, Brick brick, int z)
    {
        bool isSupported = false;
        foreach (Point point2 in brick.XYPoints())
        {
            int below = space[point2.X, point2.Y, z - 1];
            if (below > 0 || z == 1)
            {
                isSupported = true;
                break;
            }
        }

        return isSupported;
    }
    
    private static HashSet<int> SupportedBy(int[,,] space, Brick brick)
    {
        int z = brick.Start.Z;
        HashSet<int> supportedBy = [];
        foreach (Point point2 in brick.XYPoints())
        {
            int below = space[point2.X, point2.Y, z - 1];
            if (below > 0 && z > 1)
            {
                supportedBy.Add(below);
            }
        }
        return supportedBy;
    }

    private static void SetGrid(int[,,] space, Brick brick, int brickId)
    {
        foreach (var pt in brick.AllPoints())
        {
            space[pt.X, pt.Y, pt.Z] = brickId;
        }
    }

    private Brick ParseLine(string line)
    {
        LineReader reader = new(line);
        Point3 first = ParsePoint(ref reader);
        reader.ReadChar('~');
        Point3 second = ParsePoint(ref reader);

        return new Brick(first, second);
    }

    private Point3 ParsePoint(ref LineReader reader)
    {
        int x = reader.ReadInt();
        reader.ReadChar(',');
        int y = reader.ReadInt();
        reader.ReadChar(',');
        int z = reader.ReadInt();
        return new Point3(x, y, z);
    }
}    