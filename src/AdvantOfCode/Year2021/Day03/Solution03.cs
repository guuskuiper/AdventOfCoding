namespace AdventOfCode.Year2021.Day03;

[DayInfo(2021, 03)]
public class Solution03 : Solution
{
    private const string Awnser = "4138664\n4273224";

    public string Run()
    {
        List<string> lines = InputReader.ReadFileLines();

        int bitCount = lines[0].Length;

        string mostCommontStr = GetMostCommonBitString(lines, bitCount);
        string leastCommontStr = InvertBitString(mostCommontStr);

        int gammaRate = String2Int32(mostCommontStr);
        int epsionRate = String2Int32(leastCommontStr);

        string lineOxygen = FilterByCommonBit(lines, bitCount, false);
        string lineCO2 = FilterByCommonBit(lines, bitCount, true);

        int CO2 = String2Int32(lineCO2);
        int O2 = String2Int32(lineOxygen);

        return gammaRate * epsionRate + "\n" + CO2 * O2;
    }

    private string GetMostCommonBitString(List<string> lines, int bitCount)
    {
        string mostCommonBitString = "";
        int mostCommon = 0;
        int leastCommon = 0;
        for (int i = 0; i < bitCount; i++)
        {
            int rI = bitCount - i - 1;
            bool common = GetCommenBit(lines, i);
            mostCommon += (common ? 1 : 0) << rI;
            leastCommon += (common ? 0 : 1) << rI;
            mostCommonBitString += GetChar(common);
        }

        return mostCommonBitString;
    }

    private string FilterByCommonBit(List<string> lines, int bitCount, bool invert)
    {
        List<string> filterLines = lines;
        for (int i = 0; i < bitCount; i++)
        {
            bool common = GetCommenBit(filterLines, i, true);
            bool nCommon = !common;
            filterLines = Filter(filterLines, i, invert ? nCommon : common);
            if (filterLines.Count <= 1) break;
        }

        return filterLines.Single();
    }

    private bool GetBit(string number, int bit)
    {
        return number[bit] == GetChar(true);
    }

    private char GetChar(bool bit)
    {
        return bit ? '1' : '0';
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
        List<string> newLines = new ();
        foreach (string line in lines)
        {
            if (GetBit(line, bit) == value)
            {
                newLines.Add(line);
            }
        }

        return newLines;
    }

    private string InvertBitString(string bitString)
    {
        char[] inverted = new char[bitString.Length];
        for (int i = 0; i < bitString.Length; i++)
        {
            inverted[i] = GetChar(!GetBit(bitString, i));
        }

        return new string(inverted);
    }

    private int String2Int32(string number)
    {
        return Convert.ToInt32(number, 2);
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
