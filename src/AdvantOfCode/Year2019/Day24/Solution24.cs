using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day24;

[DayInfo(2019, 24)]
public class Solution24 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var gameOfLife = new GameOfLife(input);
        int partA = gameOfLife.CalcSame();

        var gameOfLife2 = new GameOfLife(input);
        int partB = gameOfLife2.Steps(200);
        return partA + "\n" + partB;
    }
}    