using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Test.Solutions;

public class Year2023DaySolutionTest : YearTests
{
    public Year2023DaySolutionTest(ITestOutputHelper testOutputHelper) : base(2023, testOutputHelper) {}
    
    [Fact] public void Day01() => AssertDay("55971", "54719");
    //[Fact] public Task Day01() => AssertDayAsync("55971", "54719");
    [Fact] public void Day02() => AssertDay("2416", "63307");
    [Fact] public void Day03() => AssertDay("550064", "85010461");
    [Fact] public void Day04() => AssertDay("28538", "9425061");
    [Fact] public void Day05() => AssertDay("178159714", "100165128");
    [Fact] public void Day06() => AssertDay("861300", "28101347");
    [Fact] public void Day07() => AssertDay("247823654", "245461700");
    [Fact] public void Day08() => AssertDay("19631", "21003205388413");
    [Fact] public void Day09() => AssertDay("2075724761", "1072");
    [Fact] public void Day10() => AssertDay("6870", "287");
    [Fact] public void Day11() => AssertDay("10289334", "649862989626");
    [Fact] public void Day12() => AssertDay("7361", "83317216247365");
    [Fact] public void Day13() => AssertDay("27505", "22906");
    [Fact] public void Day14() => AssertDay("109385", "93102");
    [Fact] public void Day15() => AssertDay("521434", "248279");
    [Fact] public void Day16() => AssertDay("8551", "8754");
    [Fact] public void Day17() => AssertDay("814", "974");
    [Fact] public void Day18() => AssertDay("34329", "42617947302920");
    [Fact] public void Day19() => AssertDay("397061", "125657431183201");
    [Fact] public void Day20() => AssertDay("856482136", "224046542165867");
    [Fact] public void Day21() => AssertDay("3751", "619407349431167");
    [Fact] public void Day22() => AssertDay("443", "69915");
    [Fact] public void Day23() => AssertDay("2254", "6394");
    [Fact] public void Day24() => AssertDay("26611", "684195328708898");
    [Fact] public void Day25() => AssertDay("568214", "");
}