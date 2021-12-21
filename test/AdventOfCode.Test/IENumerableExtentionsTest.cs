using System.Linq;
using AdventOfCode.Extentions;
using Xunit;

namespace AdventOfCode.Test;

public class IENumerableExtentionsTest
{
    [Fact]
    public void Numbers3pow3_SlouldContain27()
    {
        var numbers = Enumerable.Range(1, 3);

        var combinations = numbers.CombinationsWithRepetition(3);
        
        Assert.Equal(3*3*3, combinations.Count());
    }
    
    [Fact]
    public void Chars4pow3_SlouldContain64()
    {
        var chars = "ABCD".ToCharArray();

        var combinations = chars.CombinationsWithRepetition(3);
        
        Assert.Equal(4*4*4, combinations.Count());
    }
    
    [Fact]
    public void Permutations()
    {
        var chars = "ABCD".ToCharArray();

        var combinations = chars.GetPermutations().ToList();
        
        Assert.Equal(4*3*2*1, combinations.Count());
    }
}