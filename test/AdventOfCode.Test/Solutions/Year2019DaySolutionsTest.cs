using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Test.Solutions;

public class Year2019DaySolutionsTest : YearTests
{
    public Year2019DaySolutionsTest(ITestOutputHelper testOutputHelper) : base(2019, testOutputHelper) {}
    
    [Fact] public void Day01() => AssertDay("3375962", "5061072");
    [Fact] public void Day02() => AssertDay("12490719", "2003");
    [Fact] public void Day03() => AssertDay("273", "15622");
    [Fact] public void Day04() => AssertDay("594", "364");
    [Fact] public void Day05() => AssertDay("10987514", "14195011");
    [Fact] public void Day06() => AssertDay("162439", "367");
    [Fact] public void Day07() => AssertDay("844468", "4215746");
    [Fact] public void Day08() => AssertDay("1935", """ 
                                                     ██  ████ █    █  █ █    
                                                    █  █ █    █    █  █ █    
                                                    █    ███  █    █  █ █    
                                                    █    █    █    █  █ █    
                                                    █  █ █    █    █  █ █    
                                                     ██  █    ████  ██  ████ 

                                                    """);
    [Fact] public void Day09() => AssertDay("2436480432", "45710");
    [Fact] public void Day10() => AssertDay("319", "517");
    [Fact] public void Day11() => AssertDay("2415", """ 
                                                     ███  ████ ███  █  █ ████ █  █ ███   ██    
                                                     █  █ █    █  █ █  █    █ █  █ █  █ █  █   
                                                     ███  ███  █  █ █  █   █  █  █ █  █ █      
                                                     █  █ █    ███  █  █  █   █  █ ███  █      
                                                     █  █ █    █    █  █ █    █  █ █    █  █   
                                                     ███  █    █     ██  ████  ██  █     ██    

                                                    """);

    [Fact] public void Day12() => AssertDay("12053", "320380285873116");
    [Fact] public void Day13() => AssertDay("239", "12099");
    [Fact] public void Day14() => AssertDay("202617", "7863863");
    [Fact] public void Day15() => AssertDay("240", "322");
    [Fact] public void Day16() => AssertDay("68317988", "53850800");
    [Fact] public void Day17() => AssertDay("2660", "790595");
    [Fact] public void Day18() => AssertDay("4248", "1878");
    [Fact] public void Day19() => AssertDay("220", "10010825");
    [Fact] public void Day20() => AssertDay("556", "6532");
    [Fact] public void Day21() => AssertDay("19353074", "1147582556");
    [Fact] public void Day22() => AssertDay("8502", "41685581334351");
    [Fact] public void Day23() => AssertDay("19724", "15252");
    [Fact] public void Day24() => AssertDay("18844281", "1872");
    [Fact] public void Day25() => AssertDay("2147502592", "");
}