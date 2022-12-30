using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day23;

[DayInfo(2019, 23)]
public class Solution23 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        
        var instructions = input[0].Split(',').Select(long.Parse).ToArray();

        var network = new Network(instructions, 50);
        long result = network.Boot();
        return network.FirstPacket.Y + "\n" + result;
    }
}    