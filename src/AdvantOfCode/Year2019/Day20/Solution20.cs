using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day20;

[DayInfo(2019, 20)]
public class Solution20 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var donutMaze = new DonutMaze(input);
        int partA = donutMaze.Solve();
        int partB = donutMaze.Solve2();
        return partA + "\n" + partB;
    }
}    