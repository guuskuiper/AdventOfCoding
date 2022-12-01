using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace AdventOfCode.Test;

public class Year2022DaySolutionTest
{
    private const string Prefix = "Day";
    private const string Year = "Year2022";
    
    [Fact] public void Day01() => AssertDay("", "");

    private void AssertDay(string expectedA, string expectedB, [CallerMemberName] string callerName = "")
    {
        int day = int.Parse(callerName.Substring(Prefix.Length));
        string solutionName = $"Solution{day:D2}";
        Solution current = DayGenerator.GetByName(solutionName, Year);
        var solution = current.Run();
        Assert.Equal(expectedA + "\n" + expectedB, solution);
    }
}