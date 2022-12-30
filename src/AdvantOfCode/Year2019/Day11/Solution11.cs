using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day11;

[DayInfo(2019, 11)]
public class Solution11 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var instructions = input[0].Split(',').Select(long.Parse).ToArray();

        var robot = new Robot(instructions, 0);
        var painted = robot.StartRobot();

        robot = new Robot(instructions, 1);
        robot.StartRobot();
        string result = robot.Draw();
        
        return painted + "\n" + result;
    }
}