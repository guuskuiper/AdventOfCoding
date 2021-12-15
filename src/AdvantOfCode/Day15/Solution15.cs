using System.Text;

namespace AdventOfCode.Day15;

public class Solution15 : Solution
{
    public class Node
    {
        public bool Visited { get; set; }
        public long Distance { get; set; }
        public int X { get; init; }
        public int Y { get; init; }
    }

    public class NodeComparer : IComparer<Node>
    {
        public int Compare(Node? x, Node? y)
        {
            int compareDistance = x!.Distance.CompareTo(y!.Distance);
            if (compareDistance != 0)
            {
                return compareDistance;
            }
            
            int compareX = x!.X.CompareTo(y!.X);
            if (compareX != 0)
            {
                return compareX;
            }
            
            int compareY = x!.Y.CompareTo(y!.Y);
            if (compareY != 0)
            {
                return compareY;
            }

            return 0;
        }
    }
    
    private int[,] cave;
    private Node[,] nodes;
    private SortedSet<Node> unvisited;

    public Solution15()
    {
        var lines = InputReader.ReadFileLinesArray();
        cave = new int[lines.Length,lines[0].Length];
        nodes = new Node[lines.Length,lines[0].Length];
        for (int x = 0; x < lines.Length; x++)
        {
            for (int y = 0; y < lines[0].Length; y++)
            {
                cave[x, y] = int.Parse(lines[x][y].ToString());
            }
        }

        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                nodes[x, y] = new Node()
                {
                    Distance = long.MaxValue,
                    Visited = false,
                    X = x,
                    Y = y,
                };
            }
        }
        nodes[0, 0].Distance = 0;

        unvisited = new SortedSet<Node>( new NodeComparer());
        foreach (var node in nodes)
        {
            unvisited.Add(node);
        }
    }

    private void InitializeLargeCave(int size = 5)
    {
        int[,] newCave = new int[cave.GetLength(0) * size, cave.GetLength(1) * size];
        Node[,] newNodes = new Node[newCave.GetLength(0), newCave.GetLength(1)];
        
        for (int x = 0; x < newCave.GetLength(0); x++)
        {
            for (int y = 0; y < newCave.GetLength(1); y++)
            {
                int oldX = x % cave.GetLength(0);
                int oldY = y % cave.GetLength(1);
                int dx = x / cave.GetLength(0);
                int dy = y / cave.GetLength(1);
                int d = dx + dy;
                int newValue = Wrap(cave[oldX, oldY] + d);
                newCave[x, y] = newValue;
            }
        }
        cave = newCave;
        
        for (int x = 0; x < newNodes.GetLength(0); x++)
        {
            for (int y = 0; y < newNodes.GetLength(1); y++)
            {
                newNodes[x, y] = new Node()
                {
                    Distance = long.MaxValue,
                    Visited = false,
                    X = x,
                    Y = y,
                };
            }
        }
        nodes = newNodes;
        nodes[0, 0].Distance = 0;
        
        unvisited = new SortedSet<Node>( new NodeComparer());
        foreach (var node in nodes)
        {
            unvisited.Add(node);
        }
    }

    private int Wrap(int n)
    {
        while (n > 9)
        {
            n -= 9;
        }

        return n;
    }

    public string Run()
    {
        int endX = nodes.GetLength(0) - 1;
        int endY = nodes.GetLength(1) - 1;

        Solve(endX, endY);
        long shortestPath = nodes[endX, endY].Distance;
        
        InitializeLargeCave(5);
        endX = nodes.GetLength(0) - 1;
        endY = nodes.GetLength(1) - 1;
        Solve(endX, endY);
        long shortestPathB = nodes[endX, endY].Distance;
        
        return shortestPath + "\n" + shortestPathB; 
    }

    private void Solve(int endX, int endY)
    {
        while (true)
        {
            Node n = GetSmallestDistanceUnvisitedNode();
            UpdateNeighbours(n);

            if (n.X == endX && n.Y == endY)
            {
                break;
            }
        }
    }

    private Node GetSmallestDistanceUnvisitedNode()
    {
        return unvisited.First();
    }
    
    private void UpdateNeighbours(Node n)
    {
        UpdateNeighbour(n.X - 1, n.Y, n.Distance);
        UpdateNeighbour(n.X + 1, n.Y, n.Distance);
        UpdateNeighbour(n.X, n.Y - 1, n.Distance);
        UpdateNeighbour(n.X, n.Y + 1, n.Distance);

        n.Visited = true;
        unvisited.Remove(n);
    }
    
    private void UpdateNeighbour(int x, int y, long prevDistance)
    {
        if(!InRange(x, y)) return;

        Node n = nodes[x, y];
        
        if(n.Visited) return;

        long distance = prevDistance + cave[x, y];
        if (distance < n.Distance)
        {
            unvisited.Remove(n);
            n.Distance = distance;
            unvisited.Add(n);
        }
    }

    private bool InRange(int x, int y)
    {
        return x >= 0 && x < nodes.GetLength(0) &&
               y >= 0 && y < nodes.GetLength(1);
    }

    private void ShowVisited()
    {
        string s = PrintVisited();
        Console.WriteLine(s);
    }

    private string PrintVisited()
    {
        StringBuilder sb = new StringBuilder();
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                sb.Append(nodes[x, y].Visited ? 'X' : ' ');
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
