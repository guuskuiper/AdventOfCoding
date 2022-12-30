using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day19;

[DayInfo(2019, 19)]
public class Solution19 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var instructions = input[0].Split(',').Select(long.Parse).ToArray();

        var tractorBeam = new TractorBeam(instructions);
        int partA = tractorBeam.Count();
        int partB = tractorBeam.Follow(100);

        // Part2:
        // Follow 1 edge
        // check if the point on the diagonal is also inside

        return partA + "\n" + partB;
    }
}    