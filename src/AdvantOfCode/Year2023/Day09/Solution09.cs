using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day09;

[DayInfo(2023, 09)]
public class Solution09 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var numbers = input.Select(ParseLine).ToArray();

        var extrapolated = numbers.Select(SolveLine).ToArray();
        var sum = extrapolated.Sum();

        var extrapolatedReverse = numbers.Select(x => SolveLine(x.Reverse().ToArray()));
        var sum2 = extrapolatedReverse.Sum();
        
        return sum + "\n" + sum2;
    }
    
    private int SolveLine(int[] values)
    {
        if (values.All(x => x == 0))
        {
            return 0;
        }

        int[] diff = new int[values.Length - 1];
        for (int i = 1; i < values.Length; i++)
        {
            diff[i-1] =(values[i] - values[i - 1]);
        }

        int last = values[values.Length - 1];
        int recursive = SolveLine(diff);
        return recursive + last;
    }

    private int[] ParseLine(string line)
    {
        LineReader reader = new(line);

        List<int> numbers = new();
        
        while (!reader.IsDone)
        {
            numbers.Add(reader.ReadInt());
            reader.SkipWhitespaces();
        }

        return numbers.ToArray();
    }
}    