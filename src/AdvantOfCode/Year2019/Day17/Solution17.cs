using System.Text;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day17;

[DayInfo(2019, 17)]
public class Solution17 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var instructions = input[0].Split(',').Select(long.Parse).ToArray();

        var ascii = new ASCII(instructions);
        int partA = ascii.Start();

        // manually splitting it up into 3 parts (A, B, C) and creating the main sequence:
        // A,B,A,C,A,B,C,B,C,B
        // A: 
        // L,10,R,8,L,6,R,6

        // B:
        // L,8,L,8,R,8

        // C:
        // R,8,L,6,L,10,L,10
        var sb = new StringBuilder();
        sb.AppendLine("A,B,A,C,A,B,C,B,C,B"); // main
        sb.AppendLine("L,10,R,8,L,6,R,6"); // A
        sb.AppendLine("L,8,L,8,R,8"); // B
        sb.AppendLine("R,8,L,6,L,10,L,10"); // C
        sb.AppendLine("y");// continues feed  y/ n

        var ascii2 = new ASCII(instructions);
        ascii2.Start2(sb.ToString().Replace("\r\n", "\n"));
        return partA + "\n" + ascii2.Response;
    }
}    