using static AdventOfCode.Year2021.Day05.Solution05;
using System.Drawing;

namespace AdventOfCode.Year2022.Day06;

[DayInfo(2022, 06)]
public class Solution06 : Solution
{
    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        var line = lines[0];

        int index = IndexOfAllDifferent(line, 4);
        int indexB = IndexOfAllDifferentAlternative(line.AsSpan().Slice(index), 14) + index;

        return index + "\n" + indexB;
    }

    private int IndexOfAllDifferent(ReadOnlySpan<char> line, int size)
    {
        var set = new HashSet<char>(size);
        for (int i = size; i < line.Length; i++)
        {
            ReadOnlySpan<char> windows = line.Slice(i - size, size);
            if (IsAllDifferent(set, windows)) return i;
        }

        return -1;
    }

    private bool IsAllDifferent(HashSet<char> set, ReadOnlySpan<char> window)
    {
        set.Clear();
        foreach (var c in window)
        {
            set.Add(c);
        }
        return set.Count == window.Length;
    }

    private int IndexOfAllDifferentAlternative(ReadOnlySpan<char> line, int size)
    {
        for (int i = size; i < line.Length; i++)
        {
            ReadOnlySpan<char> windows = line.Slice(i - size, size);
            if (IsAllDifferent(windows)) return i;
        }

        return -1;
    }

    private bool IsAllDifferent(ReadOnlySpan<char> window)
    {
        for (int i = 0; i < window.Length; i++)
        {
            for (int j = i + 1; j < window.Length; j++)
            {
                if (window[i] == window[j]) return false;
            }
        }
        return true;
    }
}
