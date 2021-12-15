using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace AdventOfCode.Test;

public class DaySolutionTest
{
    private const string Prefix = "Day";
    
    [Fact] public void Day01() => AssertDay("1548", "1589", Prefix + 1);
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
    
    private void AssertDay(string expectedA, string expectedB, [CallerMemberName] string callerName = "")
    {
        int day = int.Parse(callerName.Substring(Prefix.Length));
        string solutionName = $"Solution{day:D2}";
        Solution current = DayGenerator.GetByName(solutionName);
        var solution = current.Run();
        Assert.Equal(expectedA + "\n" + expectedB, solution);
    }
}