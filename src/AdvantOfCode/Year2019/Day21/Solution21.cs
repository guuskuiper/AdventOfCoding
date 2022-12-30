using System.Text;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day21;

[DayInfo(2019, 21)]
public class Solution21 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var instructions = input[0].Split(',').Select(long.Parse).ToArray();

        var sb = new StringBuilder();
        sb.AppendLine("NOT A J"); // jump if hole in 1
        sb.AppendLine("NOT B T"); // jump if hole in 2
        sb.AppendLine("OR T J"); // !A | !B
        sb.AppendLine("NOT C T"); // jump if hole in 3
        sb.AppendLine("OR T J"); // !A | !B | !C
        sb.AppendLine("AND D J"); // jump to ground J = D & J
        sb.AppendLine("WALK"); // main

        var sprintDroid = new SpringDroid(instructions);
        sprintDroid.Start(sb.ToString().Replace("\r\n", "\n"));
        long partA = sprintDroid.OutputValue;

        var sprintDroid2 = new SpringDroid(instructions);
        var sb2 = new StringBuilder();
        sb2.AppendLine("NOT A J"); // jump if hole in 1
        sb2.AppendLine("NOT B T"); // jump if hole in 2
        sb2.AppendLine("OR T J"); // !A | !B
        sb2.AppendLine("NOT C T"); // jump if hole in 3
        sb2.AppendLine("OR T J"); // !A | !B | !C
        sb2.AppendLine("AND D J"); // jump to ground J = D & J
        sb2.AppendLine("NOT E T");
        sb2.AppendLine("AND H T");// can rejump
        sb2.AppendLine("OR E T");
        sb2.AppendLine("AND T J");
        sb2.AppendLine("RUN"); // main

        sprintDroid2.Start(sb2.ToString().Replace("\r\n", "\n"));
        long partB = sprintDroid2.OutputValue;
        return partA + "\n" + partB;
    }
}    