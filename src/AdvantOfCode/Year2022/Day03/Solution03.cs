namespace AdventOfCode.Year2022.Day03;

[DayInfo(2022, 03)]
public class Solution03 : Solution
{
    public string Run()
    {
        var lines = InputReader.ReadFileLines();

        var priorities = Priorities(lines);
        var badges = Badges(lines);
        return priorities + "\n" + badges;
    }

    private long Priorities(List<string> lines)
    {
        long priorities = 0;
        foreach (var line in lines)
        {
             char error = Union(line);
             priorities += Priority(error);
        }
        return priorities;
    }

    private static int Priority(char c) => char.IsUpper(c) ? c - 'A' + 27 : c - 'a' + 1;

    private char Union(string line)
    {
        int half = line.Length / 2;
        var first = line.Substring(0, half);
        var second = line.Substring(half);
        
        return first.Intersect(second).Single();
    }
    
    private long Badges(IEnumerable<string> lines)
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

    private char Group(IReadOnlyList<string> group)
    {
        return group[0].Intersect(group[1]).Intersect(group[2]).Single();
    }
}
