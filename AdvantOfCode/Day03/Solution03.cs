namespace AdventOfCode.Day03;

public class Solution03 : Solution
{
    public string Run()
    {
        List<string> lines = InputReader.ReadFileLines();

        int bitCount = lines[0].Length;

        var mostCommontStr = new string('-', bitCount).ToCharArray();
        var leastCommontStr = new string('-', bitCount).ToCharArray();

        int mostCommon = 0;
        int leastCommon = 0;
        for (int i = 0; i < bitCount; i++)
        {
            int rI = bitCount - i - 1;
            bool common = GetCommenBit(lines, rI);
            mostCommon += (common ? 1 : 0) << (i);
            leastCommon += (common ? 0 : 1) << (i);
            mostCommontStr[i] = (common ? '1' : '0');
            leastCommontStr[i] = (common ? '0' : '1');
        }

        int mult = mostCommon * leastCommon;

        List<string> filterLinesOxygen = lines;

        int oxygen = 0;
        for (int i = 0; i < bitCount; i++)
        {
            int rI = bitCount - i - 1;
            bool common = GetCommenBit(filterLinesOxygen, i, true);
            filterLinesOxygen = Filter(filterLinesOxygen, i, common);
            if(filterLinesOxygen.Count <= 1) break;
        }

        string lineOxygen = filterLinesOxygen.Single();

        List<string> filterLinesCO2 = lines;
        for (int i = 0; i < bitCount; i++)
        {
            int rI = bitCount - i - 1;
            bool common = GetCommenBit(filterLinesCO2, i, true);
            bool nCommon = !common;
            filterLinesCO2 = Filter(filterLinesCO2, i , nCommon);
            if (filterLinesCO2.Count <= 1) break;
        }

        string lineCO2 = filterLinesCO2.Single();

        int CO2 = String2Int(lineCO2);
        int O2 = String2Int(lineOxygen);

        return mult.ToString() + "\n" + CO2 * O2;
    }

    private bool GetBit(string number, int bit)
    {
        return number[bit] == '1';
    }

    private bool GetCommenBit(List<string> lines, int bit, bool onEqual = true)
    {
        int count = 0;
        foreach (string line in lines)
        {
            if (GetBit(line, bit))
            {
                count++;
            }
        }

        if (count * 2 == lines.Count) return onEqual;
        return (count * 2) > lines.Count;
    }

    private List<string> Filter(List<string> lines, int bit, bool value)
    {
        List<string> newLines = new List<string>();
        foreach (string line in lines)
        {
            if (GetBit(line, bit) == value)
            {
                newLines.Add(line);
            }
        }

        return newLines;
    }

    private int String2Int(string number)
    {
        int value = 0;
        for (int i = 0; i < number.Length; i++)
        {
            int rI = number.Length - i - 1;
            bool common = GetBit(number, rI);
            value += (common ? 1 : 0) << i;
        }

        return value;
    }
}
