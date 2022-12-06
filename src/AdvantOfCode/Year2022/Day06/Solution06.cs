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
        for (int i = size; i < line.Length; i++)
        {
            bool same = false;
            for (int j = 0; j < size; j++)
            {
                for (int z = j + 1; z < size; z++)
                {
                    if (line[i-j] == line[i-z])
                    {
                        same = true;
                        break;
                    }
                }
                
                if(same) break;
            }

            if (!same) return i + 1;
        }

        return -1;
    }
}
