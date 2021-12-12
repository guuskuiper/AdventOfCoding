namespace AdventOfCode.Day12;

public class Solution12 : Solution
{
    private class Node
    {
        public string Name { get; set; }
        public bool IsLarge { get; set; }
        public List<Node> Connections = new ();
    }

    private class Path
    {
        public Node? Double = null;
        public List<Node> Nodes = new ();

        public Node Current() => Nodes[^1];
        public Path AddNode(Node n)
        {
            Node? newDouble = !n.IsLarge && Nodes.Contains(n) && Double == null ? n : Double;  
            return new Path()
            {
                Double = newDouble,
                Nodes = new(Nodes) {n}
            };
        }

        public bool CanAdd(Node n)
        {
            return n.IsLarge || !Nodes.Contains(n) || Double == null;
        }
    }

    private Dictionary<string, Node> _nodes = new ();
    private List<List<Node>> _paths = new ();
    private List<Path> _pathsDouble = new();
    
    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        ParseLines(lines);
        FindAllPaths();
        long A = _paths.Count;
        FindAllPathsB();
        long B = _pathsDouble.Count;
        return A + "\n" + B;
    }
    
    private void FindAllPathsB()
    {
        Node start = _nodes["start"];
        Path path = new Path().AddNode(start);
        FindNextB(path);
    }

    private void FindNextB(Path path)
    {
        Node current = path.Current();
        foreach (var connection in current.Connections)
        {
            if(!path.CanAdd(connection)) continue;
            if(connection.Name == "start") continue;

            Path newPath = path.AddNode(connection);

            if (connection.Name == "end")
            {
                _pathsDouble.Add(newPath);
            }
            else
            {
                FindNextB(newPath);
            }
        }
    }

    private void FindAllPaths()
    {
        Node start = _nodes["start"];
        List<Node> path = new () { start};
        FindNext(path);
    }

    private void FindNext(List<Node> path)
    {
        Node current = path[^1];
        foreach (var connection in current.Connections)
        {
            if(!connection.IsLarge && path.Contains(connection)) continue;

            List<Node> newPath = new List<Node>(path) {connection};

            if (connection.Name == "end")
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
            n = new Node
            {
                Name = name,
                IsLarge = char.IsUpper(name, 0)
            };
            _nodes[name] = n;
        }

        return n;
    }
}
