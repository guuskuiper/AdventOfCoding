using System.Buffers;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2023.Day01;

[DayInfo(2023, 01)]
public class Solution01 : Solution
{
    private static readonly SearchValues<char> DigitSearchValues = SearchValues.Create("0123456789");
    
    private static readonly string[] Digits = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];
    private static readonly string[] TextDigits = ["zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
    private static readonly string[] DigitsAndTextDigits = [..Digits, ..TextDigits];
    
    public string Run()
    {
        string[] input = this.ReadLines();

        int sum = input.Select(ParseLine).Sum();
        int sumB = input.Select(ParseLineB).Sum();
        
        return sum + "\n" + sumB;
    }

    private int ParseLine(string line)
    {
        ReadOnlySpan<char> lineSpan = line.AsSpan();
        int first = lineSpan.IndexOfAny(DigitSearchValues);
        int last = lineSpan.LastIndexOfAny(DigitSearchValues);
        string number = new string([lineSpan[first], lineSpan[last]]);
        return int.Parse(number);
    }

    private int ParseLineB(string line)
    {
        string minText = DigitsAndTextDigits.Select(t => (line.IndexOf(t, StringComparison.Ordinal), t))
            .Where(p => p.Item1 >= 0)
            .MinBy(x => x.Item1)
            .Item2;
        
        string maxText = DigitsAndTextDigits.Select(t => (line.LastIndexOf(t, StringComparison.Ordinal), t))
            .Where(p => p.Item1 >= 0)
            .MaxBy(x => x.Item1)
            .Item2;

        int first = Parse(minText);
        int last = Parse(maxText);
        return first * 10 + last;
    }

    private int Parse(string s)
    {
        int number = Array.IndexOf(TextDigits, s);
        if (number < 0) number = int.Parse(s);
        return number;
    }
}    