using System;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Test.Solutions;

public class Year2021DaySolutionTest : YearTests
{
    public Year2021DaySolutionTest(ITestOutputHelper testOutputHelper) : base(2021, testOutputHelper) {}
    
    [Fact] public void Day01() => AssertDay("1548", "1589");
    [Fact] public void Day02() => AssertDay("1746616", "1741971043");
    [Fact] public void Day03() => AssertDay("4138664", "4273224");
    [Fact] public void Day04() => AssertDay("31424", "23042");
    [Fact] public void Day05() => AssertDay("6666", "19081");
    [Fact] public void Day06() => AssertDay("360268", "1632146183902");
    [Fact] public void Day07() => AssertDay("323647", "87640209");
    [Fact] public void Day08() => AssertDay("476", "1011823");
    [Fact] public void Day09() => AssertDay("500", "970200");
    [Fact] public void Day10() => AssertDay("243939", "2421222841");
    [Fact] public void Day11() => AssertDay("1675", "515");
    [Fact] public void Day12() => AssertDay("5254", "149385");
    [Fact] public void Day13() => AssertDay("684", "" +
                                                   "  ## ###  #### ###  #     ##  #  # #  # " + Environment.NewLine +
                                                   "   # #  #    # #  # #    #  # # #  #  # " + Environment.NewLine +
                                                   "   # #  #   #  ###  #    #    ##   #### " + Environment.NewLine +
                                                   "   # ###   #   #  # #    # ## # #  #  # " + Environment.NewLine +
                                                   "#  # # #  #    #  # #    #  # # #  #  # " + Environment.NewLine +
                                                   " ##  #  # #### ###  ####  ### #  # #  # " + Environment.NewLine);
    [Fact] public void Day14() => AssertDay("2345", "2432786807053");
    [Fact] public void Day15() => AssertDay("540", "2879");
    [Fact] public void Day16() => AssertDay("960", "12301926782560");
    [Fact] public void Day17() => AssertDay("2278", "996");
    [Fact] public void Day18() => AssertDay("4457", "4784");
    [Fact(Skip = "Too slow currently")] public void Day19() => AssertDay("438", "11985");
    [Fact] public void Day20() => AssertDay("5179", "16112");
    [Fact] public void Day21() => AssertDay("802452", "270005289024391");
    [Fact] public void Day22() => AssertDay("581108", "1325473814582641");
    [Fact] public void Day23() => AssertDay("19160", "47232");
    [Fact] public void Day24() => AssertDay("99893999291967", "34171911181211");
    [Fact] public void Day25() => AssertDay("582", "");
}