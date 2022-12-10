using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace AdventOfCode.Test;

public class Year2022DaySolutionTest
{
    private const string Prefix = "Day";
    private const string Year = "Year2022";
    
    [Fact] public void Day01() => AssertDay("70369", "203002");
    [Fact] public void Day02() => AssertDay("11449", "13187");
    [Fact] public void Day03() => AssertDay("7908", "2838");
    [Fact] public void Day04() => AssertDay("503", "827");
    [Fact] public void Day05() => AssertDay("TQRFCBSJJ", "RMHFJNVFP");
    [Fact] public void Day06() => AssertDay("1920", "2334");
    [Fact] public void Day07() => AssertDay("1644735", "1300850");
    [Fact] public void Day07B() => AssertDay("1644735", "1300850");
    [Fact] public void Day08() => AssertDay("1792", "334880");
    [Fact] public void Day09() => AssertDay("6181", "2386");
    [Fact] public void Day10() => AssertDay("13440", """
    ###..###..####..##..###...##..####..##..
    #..#.#..#....#.#..#.#..#.#..#....#.#..#.
    #..#.###....#..#....#..#.#..#...#..#..#.
    ###..#..#..#...#.##.###..####..#...####.
    #....#..#.#....#..#.#.#..#..#.#....#..#.
    #....###..####..###.#..#.#..#.####.#..#.
    
    """);
    
    private void AssertDay(string expectedA, string expectedB, [CallerMemberName] string callerName = "")
    {
        string day = callerName.Substring(Prefix.Length);
        string solutionName = $"Solution{day}";
        Solution current = DayGenerator.GetByName(solutionName, Year);
        var solution = current.Run();
        Assert.Equal(expectedA + "\n" + expectedB, solution);
    }
}