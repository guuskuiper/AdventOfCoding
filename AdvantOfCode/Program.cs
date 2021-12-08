using AdventOfCode;

int day = Math.Min(DateTime.Today.Day, 25);
if(args.Length > 0 && int.TryParse(args[0], out int dayArg)) day = dayArg;
string solutionName = $"Solution{day:D2}";
Solution current = DayGenerator.GetByName(solutionName);

Console.WriteLine("** AdventOfCode **");
Console.WriteLine($"* {current.GetType().Name} *");
Console.WriteLine(current.Run());

