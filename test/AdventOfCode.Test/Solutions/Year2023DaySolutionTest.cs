using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Test.Solutions;

public class Year2023DaySolutionTest : YearTests
{
    public Year2023DaySolutionTest(ITestOutputHelper testOutputHelper) : base(2023, testOutputHelper) {}
    
    [Fact] public void Day01() => AssertDay("55971", "54719");
}