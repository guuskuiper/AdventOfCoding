using AdventOfCode;

Console.WriteLine("** AdventOfCode **");
//await DayGenerator.CreateDirectoriesPerDay(true);

Solution current = DayGenerator.GetByName("Solution01");

Console.WriteLine($"* {current.GetType().Name} *");
var result = current.Run();
Console.WriteLine(result);

