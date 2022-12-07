using System.Drawing;

namespace AdventOfCode.Year2022.Day07;

public class Solution07 : Solution
{
    private const int LIMIT1 = 100_000;
    private const int TOTAL_SIZE = 70_000_000;
    private const int REQUIRED_SIZE = 30_000_000;
    
    private readonly Dictionary<string, int> _dirSize = new ();
    private readonly Stack<string> _path = new ();
    
    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        Parse(lines);

        var sum = _dirSize
            .Values
            .Where(x => x <= LIMIT1)
            .Sum();

        int unused = TOTAL_SIZE - _dirSize["/"];
        int toFree = REQUIRED_SIZE - unused;
        var dirs = _dirSize
            .Values
            .Where(x => x > toFree)
            .Order()
            .ToList();
        
        return sum + "\n" + dirs[0];
    }

    private void Parse(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            Parse(line);    
        }
    }

    private void Parse(string line)
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
                    _path.Pop();
                }
                else
                {
                    _path.Push(arg);
                    string dir = string.Join('/', _path.Reverse());
                    _dirSize[dir] = 0;
                }
            }
        }
        else if (char.IsNumber(first))
        {
            int size = int.Parse(split[0]);
            for (int i = 0; i < _path.Count; i++)
            {
                string dir = string.Join('/', _path.Reverse().Take(i + 1));
                _dirSize[dir] += size;
            }
        }
    }
}
