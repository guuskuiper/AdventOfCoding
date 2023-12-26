using System;
using System.Drawing;
using AdventOfCode.Year2022.Day22;
using Xunit;
using static AdventOfCode.Year2022.Day22.Solution22;

namespace AdventOfCode.Test.Solutions;

public class JungleMapTests
{
    private readonly JungleMap jungle;

    public JungleMapTests()
    {
        var lines = new Solution22().GetInput();
        jungle = new JungleMap(lines.AsSpan(0, 200).ToArray());
    }

    private static Size Left = new Size(-1, 0);
    private static Size Right = new Size(1, 0);
    private static Size Up = new Size(0, -1);
    private static Size Down = new Size(0, 1);

    [Fact]
    public void OneFive()
    {
        var result = jungle.TestWrap(new Point(50, 1), new Point(49, 1));
        Assert.Equal(new Point(0, 148), result.point);
        Assert.Equal(Right, result.direction);
    }

    [Fact]
    public void FiveOne()
    {
        var result = jungle.TestWrap(new Point(0, 148), new Point(-1, 148));
        Assert.Equal(new Point(50, 1), result.point);
        Assert.Equal(Right, result.direction);
    }

    [Fact]
    public void OneFour()
    {
        var result = jungle.TestWrap(new Point(51, 0), new Point(51, -1));
        Assert.Equal(new Point(0, 151), result.point);
        Assert.Equal(Right, result.direction);
    }

    [Fact]
    public void FourOne()
    {
        var result = jungle.TestWrap(new Point(0, 151), new Point(-1, 151));
        Assert.Equal(new Point(51, 0), result.point);
        Assert.Equal(Down, result.direction);
    }

    [Fact]
    public void TwoFive()
    {
        var result = jungle.TestWrap(new Point(50, 51), new Point(49, 51));
        Assert.Equal(new Point(1, 100), result.point);
        Assert.Equal(Down, result.direction);
    }

    [Fact]
    public void FiveTwo()
    {
        var result = jungle.TestWrap(new Point(1, 100), new Point(1, 99));
        Assert.Equal(new Point(50, 51), result.point);
        Assert.Equal(Right, result.direction);
    }

    [Fact]
    public void TwoThree()
    {
        var result = jungle.TestWrap(new Point(99, 51), new Point(100, 51));
        Assert.Equal(new Point(101, 49), result.point);
        Assert.Equal(Up, result.direction);
    }

    [Fact]
    public void ThreeTwo()
    {
        var result = jungle.TestWrap(new Point(101, 49), new Point(101, 50));
        Assert.Equal(new Point(99, 51), result.point);
        Assert.Equal(Left, result.direction);
    }

    [Fact]
    public void ThreeSix()
    {
        var result = jungle.TestWrap(new Point(149, 1), new Point(150, 1));
        Assert.Equal(new Point(99, 148), result.point);
        Assert.Equal(Left, result.direction);
    }

    [Fact]
    public void SixThree()
    {
        var result = jungle.TestWrap(new Point(99, 148), new Point(100, 148));
        Assert.Equal(new Point(149, 1), result.point);
        Assert.Equal(Left, result.direction);
    }

    [Fact]
    public void ThreeFour()
    {
        var result = jungle.TestWrap(new Point(101, 0), new Point(101, -1));
        Assert.Equal(new Point(1, 199), result.point);
        Assert.Equal(Up, result.direction);
    }

    [Fact]
    public void FourThree()
    {
        var result = jungle.TestWrap(new Point(1, 199), new Point(1, 200));
        Assert.Equal(new Point(101, 0), result.point);
        Assert.Equal(Down, result.direction);
    }

    [Fact]
    public void FourSix()
    {
        var result = jungle.TestWrap(new Point(49, 151), new Point(50, 151));
        Assert.Equal(new Point(51, 149), result.point);
        Assert.Equal(Up, result.direction);
    }

    [Fact]
    public void SixFour()
    {
        var result = jungle.TestWrap(new Point(51, 149), new Point(51, 150));
        Assert.Equal(new Point(49, 151), result.point);
        Assert.Equal(Left, result.direction);
    }
}
