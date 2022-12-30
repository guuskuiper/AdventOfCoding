using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day09;

[DayInfo(2019, 09)]
public class Solution09 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var instructions = input[0].Split(',').Select(long.Parse).ToArray();

        long resultA = 0;
        var elfComputer = new ElfComputer(instructions, () => 1, l => resultA = l);
        elfComputer.ProcessInstructions();
        
        long resultB = 0;
        elfComputer = new ElfComputer(instructions, () => 2, l => resultB = l);
        elfComputer.ProcessInstructions();

        return resultA + "\n" + resultB;
    }
}    