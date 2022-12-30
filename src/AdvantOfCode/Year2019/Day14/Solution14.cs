using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day14;

[DayInfo(2019, 14)]
public class Solution14 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
        var stoi = new Stoichiometry(input);
        var ore = stoi.Solve();

        var stoi2 = new Stoichiometry(input);
        var fuel = stoi2.ConsumeOre(1_000_000_000_000);

        return ore + "\n" + fuel;
    }
}    