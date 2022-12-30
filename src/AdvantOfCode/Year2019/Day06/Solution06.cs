using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day06;

[DayInfo(2019, 06)]
public class Solution06 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        
        var orbits = new Orbits(input);
        var result = orbits.GetOrbitCount();

        var count = orbits.GetPathBetween("YOU", "SAN");
        
        return result + "\n" + count;
    }
}    