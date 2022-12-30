using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day15;

[DayInfo(2019, 15)]
public class Solution15 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var instructions = input[0].Split(',').Select(long.Parse).ToArray();

        var robot = new Robot(instructions);
        try
        {
            robot.StartRobot();
        }
        catch (Exception e)
        {
        }
        

        return robot.ShortestPathLength + "\n" + robot.FillSteps;
    }
}    