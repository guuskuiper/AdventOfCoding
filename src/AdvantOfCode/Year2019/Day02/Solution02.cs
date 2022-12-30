using AdventOfCode.Extensions;
using AdventOfCode.Year2019;

namespace AdventOfCode.Year2019.Day02;

[DayInfo(2019, 02)]
public class Solution02 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();

        var instructions = input[0].Split(',').Select(long.Parse).ToArray();

        //// change input
        instructions[1] = 12;
        instructions[2] = 2;

        var elfComputer = new ElfComputer(instructions);
        elfComputer.ProcessInstructions();

        long partA = elfComputer.GetData(0);

        int partB = PartB(instructions);
        
        return partA + "\n" + partB;
    }

    private int PartB(long[] instructions)
    {
        ElfComputer elfComputer;
        for (int noun = 0; noun <= 99; noun++)
        {
            for (int verb = 0; verb <= 99; verb++)
            {
                instructions[1] = noun;
                instructions[2] = verb;

                elfComputer = new ElfComputer(instructions);
                elfComputer.ProcessInstructions();

                if (elfComputer.GetData(0) == 19690720)
                {
                    return 100 * noun + verb;
                }
            }
        }

        return 0;
    }
}    