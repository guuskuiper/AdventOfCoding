using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day05;

[DayInfo(2019, 05)]
public class Solution05 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();

        var instructions = input[0].Split(',').Select(long.Parse).ToArray();

        long resultA = 0;
        var elfComputer = new ElfComputer(instructions, () => 1, (x) => resultA = x);
        elfComputer.ProcessInstructions();

        long resultB = 0;
        var elfComputerB = new ElfComputer(instructions, () => 5, (x) => resultB = x);
        elfComputerB.ProcessInstructions();

        return resultA + "\n" + resultB;
    }
}    