using AdventOfCode;

int day = Math.Min(DateTime.Today.Day, 25);
int year = DateTime.Today.Year;
if(args.Length > 0 && int.TryParse(args[0], out int dayArg)) day = dayArg;
if(args.Length > 1 && int.TryParse(args[1], out int yearArg)) year = yearArg;
string solutionName = $"Solution{day:D2}";
string yearName = $"Year{year}";
Solution current = DayGenerator.GetByName(solutionName, yearName);

Console.WriteLine("** AdventOfCode **");
Console.WriteLine($"* {current.GetType().Name} *");
Console.WriteLine(current.Run());

