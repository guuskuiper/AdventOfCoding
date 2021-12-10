using Xunit;

namespace AdventOfCode.Test;

public class DaySolutionTest
{
    [Fact] public void Day01() => AssertDay(1, "1548", "1589");
    [Fact] public void Day02() => AssertDay(2, "1746616", "1741971043");
    [Fact] public void Day03() => AssertDay(3, "4138664", "4273224");
    [Fact] public void Day04() => AssertDay(4, "31424", "23042");
    [Fact] public void Day05() => AssertDay(5, "6666", "19081");
    [Fact] public void Day06() => AssertDay(6, "360268", "1632146183902");
    [Fact] public void Day07() => AssertDay(7, "323647", "87640209");
    [Fact] public void Day08() => AssertDay(8, "476", "1011823");
    [Fact] public void Day09() => AssertDay(9, "500", "970200");
    [Fact] public void Day10() => AssertDay(10, "243939", "2421222841");

    private void AssertDay(int day, string expectedA, string expectedB)
    {
        string solutionName = $"Solution{day:D2}";
        Solution current = DayGenerator.GetByName(solutionName);
        var solution = current.Run();
        Assert.Equal(expectedA + "\n" + expectedB, solution);
    }
}