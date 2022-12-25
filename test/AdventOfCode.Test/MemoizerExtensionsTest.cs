using System;
using AdventOfCode.Extensions;
using Xunit;

namespace AdventOfCode.Test;

public class MemoizerExtensionsTest
{
    [Fact]
    public void Fib3_ShouldReturn2()
    {
        int fib = Fib(3);
        Assert.Equal(2, fib);
    }
    
    [Fact]
    public void Fib4_ShouldReturn3()
    {
        int fib = Fib(4);
        Assert.Equal(3, fib);
    }
    
    [Fact]
    public void Fib3Memoizer_ShouldReturn2()
    {
        Func<int, int> fib = null;
        fib = MemoizerExtension.Memoize<int, int>(n => Fib(n, fib!));

        int fibResult = fib(3);
        
        Assert.Equal(2, fibResult);
    }
    
    [Fact]
    public void Fib4Memoizer_ShouldReturn3()
    {
        Func<int, int> fib = null;
        fib = MemoizerExtension.Memoize<int, int>(n => Fib(n, fib!));

        int fibResult = fib(4);
        
        Assert.Equal(3, fibResult);
    }

    private int Fib(int n) => Fib(n, Fib);

    private int Fib(int n, Func<int, int> fib) => n > 2 ? fib(n - 1) + fib(n - 2) : 1;
}