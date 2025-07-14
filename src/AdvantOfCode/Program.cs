using System.Diagnostics;
using AdventOfCode;

int day = Math.Min(DateTime.Today.Day, 25);
int year = DateTime.Today.Year;
if(args.Length > 0 && int.TryParse(args[0], out int dayArg)) day = dayArg;
if(args.Length > 1 && int.TryParse(args[1], out int yearArg)) year = yearArg;
string solutionName = $"Solution{day:D2}";
string yearName = $"Year{year}";
SolutionAsync current = DayGenerator.GetAsyncByName(solutionName, yearName);

Console.WriteLine($"** AdventOfCode - {year} **");
Console.WriteLine($"* {current.GetType().Name} *");
long timestamp = Stopwatch.GetTimestamp();
Console.WriteLine(await current.RunAsync());
TimeSpan elapsedTime = Stopwatch.GetElapsedTime(timestamp);
Console.WriteLine($"Runtime: {elapsedTime}");
