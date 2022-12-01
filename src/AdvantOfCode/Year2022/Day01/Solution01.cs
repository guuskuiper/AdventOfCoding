namespace AdventOfCode.Year2022.Day01;

public class Solution01 : Solution
{
    public string Run()
    {
        string[] input = InputReader.ReadFile().Replace("\r\n", "\n").Split('\n');

        List<long> elves = ParseLines(input);
        
        elves.Sort();
        elves.Reverse();
        
        long max = elves[0];
        long topThree = elves.Take(3).Sum();
        
        return max + "\n" + topThree;
    }

    private List<long> ParseLines(Span<string> lines)
    {
        long currentElve = 0;
        List<long> elves = new();
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                elves.Add(currentElve);
                currentElve = 0;
                continue;
            }

            if (long.TryParse(line, out long calories))
            {
                currentElve += calories;
            }
        }

        return elves;
    }
}
