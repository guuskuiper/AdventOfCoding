using AdventOfCode;

Console.WriteLine("** AdventOfCode **");

Solution current = DayGenerator.GetByName("Solution01");

Console.WriteLine($"* {current.GetType().Name} *");
current.Run();
