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
}