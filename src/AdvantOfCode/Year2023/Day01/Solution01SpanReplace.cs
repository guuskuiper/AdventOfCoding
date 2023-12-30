using System.Buffers;

namespace AdventOfCode.Year2023.Day01;

[DayInfo(2023, 01)]
public class Solution01SpanReplace : Solution
{
    private static readonly SearchValues<char> DigitSearchValues = SearchValues.Create("0123456789");
    private static readonly Dictionary<string, string> Map = new()
    {
        { "zero", "ze0o" },
        { "one", "o1e" },
        { "two", "t2o" },
        { "three", "th3ee" },
        { "four", "fo4r" },
        { "five", "fi5e" },
        { "six", "s6x" },
        { "seven", "se7en" },
        { "eight", "ei8ht" },
        { "nine", "ni9e" },
    };
    
    public string Run()
    {
        string[] input = this.ReadLines();

        int sum = input.Select(ToMap).Select(ParseLine).Sum();
        int sumB = input.Select(ToMap).Select(Transform).Select(ParseLine).Sum();

        return sum + "\n" + sumB;
    }

    private char[] ToMap(string str) => str.ToCharArray();

    private char[] Transform(char[] input)
    {
        Span<char> span = input;

        foreach (var (textFrom, textTo) in Map)
        {
            ReplaceAll(span, textFrom, textTo);
        }
        
        return input;
    }

    private void ReplaceAll(Span<char> text, ReadOnlySpan<char> from, ReadOnlySpan<char> to)
    {
        Span<char> current = text;
        while (true)
        {
            int index = current.IndexOf(from);
            if (index < 0) break;
            
            to.CopyTo(current.Slice(index));
            current = current.Slice(index + from.Length);
        }
    }

    private int ParseLine(char[] chars)
    {
        ReadOnlySpan<char> line = chars;
        int first = line.IndexOfAny(DigitSearchValues);
        int last = line.LastIndexOfAny(DigitSearchValues);
        Span<char> output = [line[first], line[last]];
        return int.Parse(output);
    }
}    