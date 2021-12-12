namespace AdventOfCode.Day12;

public class Solution12 : Solution
{
    private class Node
    {
        public Node(string name)
        {
            Name = name;
            IsLarge = char.IsUpper(Name, 0);
        }

        public string Name { get; init; }
        public bool IsLarge { get; init; }
        public readonly List<Node> Connections = new ();
    }

    private class PathBase
    {
        protected List<Node> Nodes = new ();
        public Node Current() => Nodes[^1];
        
        public virtual PathBase AddNode(Node n)
        {
            return new PathBase()
            {
                Nodes = new(Nodes) {n}
            };
        }

        public virtual bool CanAdd(Node n)
        {
            return n.IsLarge || !Nodes.Contains(n);
        }
    }

    private class PathDouble : PathBase
    {
        private Node? _double = null;

        public override PathDouble AddNode(Node n)
        {
            Node? newDouble = !n.IsLarge && Nodes.Contains(n) && _double == null ? n : _double;  
            return new PathDouble
            {
                _double = newDouble,
                Nodes = new(Nodes) {n}
            };
        }

        public override bool CanAdd(Node n)
        {
            return n.IsLarge || !Nodes.Contains(n) || _double == null;
        }
    }

    private const string START = "start";
    private const string END = "end";
    private Dictionary<string, Node> _nodes = new ();
    private List<PathBase> _paths = new ();
    
    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        ParseLines(lines);
        
        FindAllPaths(new PathBase());
        long A = _paths.Count;
        
        FindAllPaths(new PathDouble());
        long B = _paths.Count;
        
        return A + "\n" + B;
    }

    private void FindAllPaths(PathBase root)
    {
        _paths = new();
        
        Node start = _nodes[START];
        PathBase path = root.AddNode(start);
        FindNext(path);
    }

    private void FindNext(PathBase path)
    {
        Node current = path.Current();
        foreach (var connection in current.Connections)
        {
            if (!path.CanAdd(connection) ||
                connection.Name == START)
            {
                continue;
            }

            PathBase newPath = path.AddNode(connection);

            if (connection.Name == END)
            {
                _paths.Add(newPath);
            }
            else
            {
                FindNext(newPath);
            }
        }
    }

    private void ParseLines(List<string> lines)
    {
        foreach (var line in lines)
        {
            ParseLine(line);
        }
    }

    private void ParseLine(string line)
    {
        var path = line.Split('-');
        var start = path[0];
        var end = path[1];

        Node s = CreateWhenNew(start);
        Node e = CreateWhenNew(end);
        s.Connections.Add(e);
        e.Connections.Add(s);
    }

    private Node CreateWhenNew(string name)
    {
        Node n;
        if (_nodes.ContainsKey(name))
        {
            n = _nodes[name];
        }
        else
        {
            n = new Node(name);
            _nodes[name] = n;
        }

        return n;
    }
}
