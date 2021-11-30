using System.Runtime.CompilerServices;

namespace AdventOfCode;

public static class DayGenerator
{
    public static async Task CreateDirectoriesPerDay()
    {
        var sourcePath = GetSourceFilePathName();
        var root = Path.GetDirectoryName(sourcePath);
        Console.WriteLine($"Current: {root}");
        foreach (var day in Enumerable.Range(1, 25))
        {
            var dayString = $"Day{day:D2}";
            var newDayPath = Path.Combine(root, dayString);
            if (!Directory.Exists(newDayPath))
            {
                Directory.CreateDirectory(newDayPath);
            }

            var className = $"Solution{day:D2}";
            var fileName = Path.Combine(newDayPath, $"Solution{day:D2}" + ".cs");
            string content = @""
                             + $"namespace AdventOfCode.{dayString};" + Environment.NewLine
                             + Environment.NewLine
                             + $"public class {className} : Solution" + Environment.NewLine
                             + "{" + Environment.NewLine
                             + "    public void Run()" + Environment.NewLine
                             + "    {" + Environment.NewLine
                             + "    }" + Environment.NewLine
                             + "}" + Environment.NewLine;
            await File.WriteAllTextAsync(fileName, content);
        }
    }

    static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) //
        => callerFilePath ?? "";
}