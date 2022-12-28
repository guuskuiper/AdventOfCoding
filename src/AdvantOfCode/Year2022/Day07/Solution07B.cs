using System.Drawing;

namespace AdventOfCode.Year2022.Day07;

[DayInfo(2022, 07)]
public class Solution07B : Solution
{
    private const int LIMIT1 = 100_000;
    private const int TOTAL_SIZE = 70_000_000;
    private const int REQUIRED_SIZE = 30_000_000;
    
    public string Run()
    {
        var lines = Solution07.Input();
        Parse(lines);

        RecurseTree(_root);
        var sum = _nodeSizes.Where(x => x.Key is DirNode && x.Value <= LIMIT1).Sum(x => x.Value);
        
        int unused = TOTAL_SIZE - _nodeSizes[_root];
        int toFree = REQUIRED_SIZE - unused;
        
        var smallest = _nodeSizes
            .Where(x => x.Key is DirNode && x.Value > toFree)
            .Select(x => x.Value)
            .Order()
            .First();
        
        return sum + "\n" + smallest;
    }

    private void Parse(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            ParseTree(line);
        }
    }

    private class Node
    {
        public readonly DirNode? Parent;
        public readonly string Name;

        public Node(DirNode? parent, string name)
        {
            Parent = parent;
            Name = name;
        }
    }

    private class DirNode : Node
    {
        public DirNode(DirNode? parent, string name) : base(parent, name) { }
        public List<Node> Children = new();
    }

    private class FileNode : Node
    {
        public FileNode(DirNode parent, string name, int size) : base(parent, name) => Size = size;
        public readonly int Size;
    }

    private Node _root;
    private DirNode _currentDir;
    private Dictionary<Node, int> _nodeSizes = new();

    private int RecurseTree(Node node)
    {
        int size = 0;
        if (node is FileNode fn)
        {
            size = fn.Size;
        }
        else if(node is DirNode dn)
        {
            foreach (var child in dn.Children)
            {
                size += RecurseTree(child);
            }
        }

        _nodeSizes[node] = size;

        return size;
    }

    private void ParseTree(string line)
    {
        string[] split = line.Split(' ');
        char first = line[0];
        if (first == '$')
        {
            string cmd = split[1];
            if (cmd == "cd")
            {
                string arg = split[2];
                if (arg == "..")
                {
                    _currentDir = _currentDir.Parent!;
                }
                else if(arg == "/")
                {
                    _currentDir = new DirNode(null, "/");
                    _root = _currentDir;
                }
                else
                {
                    _currentDir = (DirNode)_currentDir.Children.First(x => x.Name == arg);
                }
            }
        }
        else if (char.IsNumber(first))
        {
            string name = split[1];
            int size = int.Parse(split[0]);
            FileNode node = new FileNode(_currentDir, name, size);
            _currentDir.Children.Add(node);
        }
        else
        {
            string name = split[1];
            DirNode node = new DirNode(_currentDir, name);
            _currentDir.Children.Add(node);
        }
    }
}
