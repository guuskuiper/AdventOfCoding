using System.Reflection;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Extensions;

public static class SolutionExtensions
{
    public static string[] ReadLines(this Solution solution, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries, [CallerFilePath] string path = "")
    {
        DayInfoAttribute? dayInfo = GetDayInfo(solution);
        ArgumentNullException.ThrowIfNull(dayInfo, "No DayInfo attribute found on solution");

        string inputFileName = $"input{dayInfo.Day:D2}.txt";

        string root = RelativeToRoot(path);
        string relativePath = Path.Combine(root, "input", dayInfo.Year.ToString(), inputFileName);
        string newInputPath = Path.Combine(Path.GetDirectoryName(path)!, relativePath);
        string file = InputReader.ReadFile(dayInfo.Year, dayInfo.Day, newInputPath);
        string symlinkPath = Path.Combine(Path.GetDirectoryName(path)!, inputFileName);
        
        if (!File.Exists(symlinkPath))
        {
            File.CreateSymbolicLink(symlinkPath, relativePath);
        }

        return file
            .ReplaceLineEndings()
            .Split(Environment.NewLine, options);
    }
    
    public static async Task<string[]> ReadLinesAsync(this SolutionAsync solution, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries, [CallerFilePath] string path = "")
    {
        DayInfoAttribute? dayInfo = GetDayInfo(solution);
        ArgumentNullException.ThrowIfNull(dayInfo, "No DayInfo attribute found on solution");

        string inputFileName = $"input{dayInfo.Day:D2}.txt";

        string root = RelativeToRoot(path);
        string relativePath = Path.Combine(root, "input", dayInfo.Year.ToString(), inputFileName);
        string newInputPath = Path.Combine(Path.GetDirectoryName(path)!, relativePath);
        string file = await InputReader.ReadFileAsync(dayInfo.Year, dayInfo.Day, newInputPath);
        string symlinkPath = Path.Combine(Path.GetDirectoryName(path)!, inputFileName);
        
        if (!File.Exists(symlinkPath))
        {
            File.CreateSymbolicLink(symlinkPath, relativePath);
        }

        return file
            .ReplaceLineEndings()
            .Split(Environment.NewLine, options);
    }
    
    private static DayInfoAttribute? GetDayInfo(this Solution solution) =>
        solution.GetType().GetCustomAttribute<DayInfoAttribute>();

    private static DayInfoAttribute? GetDayInfo(this SolutionAsync solution) =>
        solution.GetType().GetCustomAttribute<DayInfoAttribute>();
    
    private static string RelativeToRoot(string fromPath)
    {
        var directoryInfo = Directory.GetParent(fromPath);
        int up = 1;
        while (directoryInfo!.Name != "src")
        {
            up++;
            directoryInfo = directoryInfo.Parent;
        }
        return Path.Combine(Enumerable.Repeat("..", up).ToArray());
    }
}