using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Test.Solutions;

public abstract class YearTests
{
    private const string Prefix = "Day";

    private readonly ITestOutputHelper _testOutputHelper;
    private readonly int _year;

    protected YearTests(int year, ITestOutputHelper testOutputHelper)
    {
        _year = year;
        _testOutputHelper = testOutputHelper;
    }
    
    protected Task AssertDay(string expectedA, string expectedB, [CallerMemberName] string callerName = "")
    {
        string day = callerName.Substring(Prefix.Length);
        int dayNumber = int.Parse(day);
        List<Solution> daySolutions = DayGenerator.GetSolutionsByDay(_year, dayNumber).ToList();
        Assert.NotEmpty(daySolutions);
        foreach (Solution daySolution in daySolutions)
        {
            string className = daySolution.GetType().Name;
            _testOutputHelper.WriteLine($"Found solution: {className}");
            long timestamp = Stopwatch.GetTimestamp();
            string solution = daySolution.Run();
            TimeSpan elapsedTime = Stopwatch.GetElapsedTime(timestamp);
            _testOutputHelper.WriteLine($"Runtime: {elapsedTime}");
            if (string.IsNullOrEmpty(expectedB))
            {
                Assert.Equal(expectedA, solution.Trim('\n'));
            }
            else
            {
                Assert.Equal(expectedA + "\n" + expectedB, solution);
            }
        }
        return Task.CompletedTask;
    }
    
    protected async Task AssertDayAsync(string expectedA, string expectedB, [CallerMemberName] string callerName = "")
    {
        string day = callerName.Substring(Prefix.Length);
        int dayNumber = int.Parse(day);
        List<SolutionAsync> daySolutions = DayGenerator.GetSolutionAsyncByDay(_year, dayNumber).ToList();
        Assert.NotEmpty(daySolutions);
        foreach (SolutionAsync daySolution in daySolutions)
        {
            string className = daySolution.GetType().Name;
            _testOutputHelper.WriteLine($"Found solution: {className}");
            long timestamp = Stopwatch.GetTimestamp();
            
            string solution = await daySolution.RunAsync();
            
            TimeSpan elapsedTime = Stopwatch.GetElapsedTime(timestamp);
            _testOutputHelper.WriteLine($"Runtime: {elapsedTime}");
            if (string.IsNullOrEmpty(expectedB))
            {
                Assert.Equal(expectedA, solution.Trim('\n'));
            }
            else
            {
                Assert.Equal(expectedA + "\n" + expectedB, solution);
            }
        }
    }
}