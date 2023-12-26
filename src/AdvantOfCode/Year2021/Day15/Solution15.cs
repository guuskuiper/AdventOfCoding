using System.Text;

namespace AdventOfCode.Year2021.Day15;

[DayInfo(2021, 15)]
public class Solution15 : Solution
{
    public class Node
    {
        public bool Visited { get; set; }
        public long Distance { get; set; }
        public int X { get; init; }
        public int Y { get; init; }
        public Node? Previous { get; set; }
    }

    private int[,] cave;
    private Node[,] nodes;
    private PriorityQueue<Node, long> _priorityQueue;

    public Solution15()
    {
        string[] lines = this.ReadLines();
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
        Node start = nodes[0, 0];
        start.Distance = 0;

        _priorityQueue = new PriorityQueue<Node, long>();
        _priorityQueue.Enqueue(start, start.Distance);
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
        Node start = nodes[0, 0];
        start.Distance = 0;

        _priorityQueue = new PriorityQueue<Node, long>();
        _priorityQueue.Enqueue(start, start.Distance);
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
        var path = Path(endX, endY);
        //Console.WriteLine(PrintPath(path));
        
        InitializeLargeCave(5);
        endX = nodes.GetLength(0) - 1;
        endY = nodes.GetLength(1) - 1;
        Solve(endX, endY);
        long shortestPathB = nodes[endX, endY].Distance;

        return shortestPath + "\n" + shortestPathB; 
    }

    private List<Node> Path(int endX, int endY)
    {
        Node current = nodes[endX, endY];
        List<Node> path = new() { current };

        while (current.Previous is not null)
        {
            current = current.Previous;
            path.Add(current);
        }

        path.Reverse();

        return path;
    }

    private void Solve(int endX, int endY)
    {
        while (true)
        {
            Node n = _priorityQueue.Dequeue();
            UpdateNeighbours(n);

            if (n.X == endX && n.Y == endY)
            {
                break;
            }
        }
    }
    
    private void UpdateNeighbours(Node n)
    {
        UpdateNeighbour(n.X - 1, n.Y, n);
        UpdateNeighbour(n.X + 1, n.Y, n);
        UpdateNeighbour(n.X, n.Y - 1, n);
        UpdateNeighbour(n.X, n.Y + 1, n);

        n.Visited = true;
    }
    
    private void UpdateNeighbour(int x, int y, Node previous)
    {
        if(!InRange(x, y)) return;

        Node n = nodes[x, y];
        
        if(n.Visited) return;

        long distance = previous.Distance + cave[x, y];
        if (distance < n.Distance)
        {
            n.Distance = distance;
            n.Previous = previous;
            _priorityQueue.Enqueue(n, n.Distance);
        }
    }

    private bool InRange(int x, int y)
    {
        return x >= 0 && x < nodes.GetLength(0) &&
               y >= 0 && y < nodes.GetLength(1);
    }
    
    private string PrintPath(List<Node> path)
    {
        char[,] grid = new char[nodes.GetLength(0), nodes.GetLength(1)];
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                grid[x, y] = ' ';
            }
        }

        foreach (var node in path)
        {
            grid[node.X, node.Y] = 'X';
        }

        StringBuilder sb = new StringBuilder();
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                sb.Append(grid[x, y]);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
