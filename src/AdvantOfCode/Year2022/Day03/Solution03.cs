namespace AdventOfCode.Year2022.Day03;

public class Solution03 : Solution
{
    private HashSet<char> _firstCompartment = new();
    private HashSet<char> _secondCompartment = new();

    public string Run()
    {
        var lines = InputReader.ReadFileLines();

        var priorities = Priorities(lines);
        var badges = Badges(lines);
        return priorities + "\n" + badges;
    }

    private long Priorities(List<string> lines)
    {
        List<char> errorTypes = new();
        long priorities = 0;
        foreach (var line in lines)
        {
             char error = Union(line);
             errorTypes.Add(error);
        }

        foreach (var error in errorTypes)
        {
            priorities += Priority(error);
        }

        return priorities;
    }

    private int Priority(char c) => char.IsUpper(c) ? c - 'A' + 27 : c - 'a' + 1;

    private char Union(ReadOnlySpan<char> line)
    {
        _firstCompartment.Clear();
        _secondCompartment.Clear();

        int half = line.Length / 2;
        var first = line.Slice(0, half);
        var second = line.Slice(half);

        foreach (var c in first)
        {
            _firstCompartment.Add(c);
        }

        foreach (var c in second)
        {
            _secondCompartment.Add(c);
        }

        var union = _firstCompartment.Intersect(_secondCompartment).Single();
        return union;
    }
    
    private long Badges(List<string> lines)
    {
        long priorities = 0;

        var groups = lines.Chunk(3);
        foreach (var group in groups)
        {
            char badge = Group(group);
            int priority = Priority(badge);
            priorities += priority;
        }
        
        return priorities;
    }

    private char Group(string[] group)
    {
        HashSet<char> one = new();
        HashSet<char> two = new();
        HashSet<char> three = new();

        foreach (var c in group[0])
        {
            one.Add(c);
        }
        foreach (var c in group[1])
        {
            two.Add(c);
        }
        foreach (var c in group[2])
        {
            three.Add(c);
        }
        
        var union = one.Intersect(two).Intersect(three).Single();
        return union;
    }

}
