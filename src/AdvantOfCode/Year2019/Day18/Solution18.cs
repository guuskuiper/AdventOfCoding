using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day18;

[DayInfo(2019, 18)]
public class Solution18 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();

        var maze = new Maze(input);
        //maze.SolveAllCombinations();
        int partA = maze.Solve();
        
        maze = new Maze(input);
        int partB = maze.Solve2();

        return partA + "\n" + partB;
    }
}    