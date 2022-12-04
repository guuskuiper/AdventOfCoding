using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace AdventOfCode.Test;

public class Year2022DaySolutionTest
{
    private const string Prefix = "Day";
    private const string Year = "Year2022";
    
    [Fact] public void Day01() => AssertDay("70369", "203002");
    [Fact] public void Day02() => AssertDay("11449", "13187");
    [Fact] public void Day03() => AssertDay("7908", "2838");

    private void AssertDay(string expectedA, string expectedB, [CallerMemberName] string callerName = "")
    {
        int day = int.Parse(callerName.Substring(Prefix.Length));
        string solutionName = $"Solution{day:D2}";
        Solution current = DayGenerator.GetByName(solutionName, Year);
        var solution = current.Run();
        Assert.Equal(expectedA + "\n" + expectedB, solution);
    }
}