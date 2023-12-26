namespace AdventOfCode.Year2021.Day02;

[DayInfo(2021, 02)]
public class Solution02 : Solution
{
    private int horiz = 0;
    private int depth = 0;
    private int aim = 0;

    public string Run()
    {
        string[] lines = this.ReadLines();

        string a = Parse(lines, this.ParseLineA);
        string b = Parse(lines, this.ParseLineB);

        return a + "\n" + b;
    }

    private string Parse(IEnumerable<string> lines, Action<string> func)
    {
        this.depth = 0;
        this.horiz = 0;
        this.aim = 0;

        foreach (string line in lines)
        {
            func(line);
        }

        return (this.horiz * this.depth).ToString();
    }

    private void ParseLineA(string line)
    {
        if (!GetValue(line, out string name, out int value)) return;

        (int dHoriz, int dDepth) = name switch
        {
            "forward" => (value, 0),
            "down" => (0, +value),
            "up" => (0, -value),
            _ => throw new ArgumentOutOfRangeException()
        };
        this.horiz += dHoriz;
        this.depth += dDepth;
    }

    private void ParseLineB(string line)
    {
        if (!GetValue(line, out string name, out int value)) return;

        (int dHoriz, int dDepth, int daim) = name switch
        {
            "forward" => (value, this.aim * value, 0),
            "down"    => (0, 0, +value),
            "up"      => (0, 0, -value),
            _ => throw new ArgumentOutOfRangeException()
        };
        this.horiz += dHoriz;
        this.depth += dDepth;
        this.aim += daim;
    }

    private static bool GetValue(string line, out string name, out int value)
    {
        var split = line.Split(' ');
        if (split.Length != 2)
        {
            name = "";
            value = int.MaxValue;
            return false;
        }

        name = split[0];
        value = int.Parse(split[1]);
        return true;
    }
}
