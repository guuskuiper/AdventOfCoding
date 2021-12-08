using AdventOfCode;

Console.WriteLine("** AdventOfCode **");
//await DayGenerator.CreateDirectoriesPerDay(true);

int day = args.Length > 0 ? int.Parse(args[0]) : 8;
string solutionName = $"Solution{day:D2}";
Solution current = DayGenerator.GetByName(solutionName);

Console.WriteLine($"* {current.GetType().Name} *");
var result = current.Run();
Console.WriteLine(result);

