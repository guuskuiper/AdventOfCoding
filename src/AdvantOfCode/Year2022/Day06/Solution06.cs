namespace AdventOfCode.Year2022.Day06;

public class Solution06 : Solution
{
    public string Run()
    {
        var lines = InputReader.ReadFileLines();
        var line = lines[0];

        int index = IndexOfAllDifferent(line, 4);
        int indexB = IndexOfAllDifferent(line.AsSpan().Slice(index), 14) + index;

        return index + "\n" + indexB;
    }

    private int IndexOfAllDifferent(ReadOnlySpan<char> line, int size)
    {
        var set = new HashSet<char>(size);
        for (int i = size; i < line.Length; i++)
        {
            set.Clear();
            ReadOnlySpan<char> windows = line.Slice(i - size, size);
            foreach (var c in windows)
            {
                set.Add(c);
            }
            if (set.Count == size) return i;
        }

        return -1;
    }
}
