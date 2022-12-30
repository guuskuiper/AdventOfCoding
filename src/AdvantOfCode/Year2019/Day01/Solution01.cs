using AdventOfCode.Extensions;

namespace AdventOfCode.Year2019.Day01;

[DayInfo(2019, 01)]
public class Solution01 : Solution
{
    public string Run()
    {
        string[] input = this.ReadLines();
    
        var sum = 0;
        foreach (var line in input)
        {
            var res = ProcessLine(line);
            var cov = Convert(res);
            sum += cov;
        }

        var fuelForFuel = 0;
        foreach (var line in input)
        {
            var res = ProcessLine(line);
            var cov = RecursiveFuel(res);
            fuelForFuel += cov;
        }

        return sum + "\n" + fuelForFuel;
    }

    private int ProcessLine(string line)
    {
        return int.Parse(line);
    }

    private int Convert(int mass)
    {
        var fuel = (int)Math.Floor((double)mass / 3) - 2;
        return fuel;
    }

    private int RecursiveFuel(int fuel)
    {
        var additionalFuel = Convert(fuel);
        if (additionalFuel >= 0)
        {
            additionalFuel += RecursiveFuel(additionalFuel);
        }
        else
        {
            return 0;
        }
        return additionalFuel;
    }
}    