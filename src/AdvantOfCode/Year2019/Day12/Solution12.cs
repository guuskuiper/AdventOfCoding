using System.Text.RegularExpressions;
using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day12;

[DayInfo(2019, 12)]
public class Solution12 : Solution
{
    public string Run()
    {
        string input = InputReader.ReadFile();
        string pattern = @"<x=(?'x'.+), y=(?'y'.+), z=(?'z'.+)>";

        List<IVector3> bodies = new List<IVector3>();

        RegexOptions options = RegexOptions.Multiline;
        
        foreach (Match m in Regex.Matches(input, pattern, options))
        {
            var x = int.Parse(m.Groups["x"].Value);
            var y = int.Parse(m.Groups["y"].Value);
            var z = int.Parse(m.Groups["z"].Value);
            var body = new IVector3(x, y, z);
            bodies.Add(body);
        }

        var nbody = new NBody(bodies);
        int partA = nbody.Simulate(1000);

        var nbody2 = new NBody(bodies);
        long partB = nbody2.SimulateUntillRepeat();
        return partA + "\n" + partB;
    }
}    