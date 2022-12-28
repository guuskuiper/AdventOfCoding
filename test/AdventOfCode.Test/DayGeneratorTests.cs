using Xunit;

namespace AdventOfCode.Test;

public class DayGeneratorTests
{
    [Fact]
    public void Get202201()
    {
        Solution solution = DayGenerator.GetSolutionByDay(2022, 01);
        Assert.NotNull(solution);
    }

    [Fact]
    public void FillTemplate()
    {
        string code = DayGenerator.FillSolutionTemplate(2022, 25);
        Assert.NotEmpty(code);
    }
}