// See https://aka.ms/new-console-template for more information

using System.Net.Mime;
using System.Runtime.CompilerServices;

Console.WriteLine("AdventOfCode");

await CreateDirectoriesPerDay();

async Task CreateDirectoriesPerDay()
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
                         + $"public class {className}" + Environment.NewLine
                         + "{" + Environment.NewLine
                         + "}" + Environment.NewLine;
        await File.WriteAllTextAsync(fileName, content);
    }
}

static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) //
    => callerFilePath ?? "";