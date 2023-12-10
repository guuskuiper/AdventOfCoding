using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Test.Solutions;

public class Year2023DaySolutionTest : YearTests
{
    public Year2023DaySolutionTest(ITestOutputHelper testOutputHelper) : base(2023, testOutputHelper) {}
    
    [Fact] public void Day01() => AssertDay("55971", "54719");
    [Fact] public void Day02() => AssertDay("2416", "63307");
    [Fact] public void Day03() => AssertDay("550064", "85010461");
    [Fact] public void Day04() => AssertDay("28538", "9425061");
    [Fact] public void Day05() => AssertDay("178159714", "100165128");
    [Fact] public void Day06() => AssertDay("861300", "28101347");
    [Fact] public void Day07() => AssertDay("247823654", "245461700");
    [Fact] public void Day08() => AssertDay("19631", "21003205388413");
    [Fact] public void Day09() => AssertDay("2075724761", "1072");
    [Fact] public void Day10() => AssertDay("6870", "287");
}