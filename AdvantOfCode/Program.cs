using AdventOfCode;

Console.WriteLine("** AdventOfCode **");
//await DayGenerator.CreateDirectoriesPerDay(true);

Solution current = DayGenerator.GetByName("Solution08");

Console.WriteLine($"* {current.GetType().Name} *");
var result = current.Run();
Console.WriteLine(result);

