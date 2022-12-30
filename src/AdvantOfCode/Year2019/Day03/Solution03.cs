using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day03;

[DayInfo(2019, 03)]
public class Solution03 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();

        var wires = new Wires(input);
        return wires.MinManhattanDist + "\n" + wires.MinTotDist;
    }
}    