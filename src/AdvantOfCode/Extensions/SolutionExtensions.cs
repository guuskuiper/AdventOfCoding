using System.Reflection;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Extensions;

public static class SolutionExtensions
{
    public static string[] ReadLines(this Solution solution, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries, [CallerFilePath] string path = "")
    {
        string inputPath = path.Replace("Solution", "input").Replace(".cs", ".txt");

        DayInfoAttribute? dayInfo = GetDayInfo(solution);
        ArgumentNullException.ThrowIfNull(dayInfo, "No DayInfo attribute found on solution");

        string newInputPath = Path.GetFullPath(
            Path.Combine(CallingFilePath(), "..", "..", "..", "input", dayInfo.Year.ToString(),
            $"input{dayInfo.Day:D2}.txt"));
        string file = InputReader.ReadFile(dayInfo.Year, dayInfo.Day, newInputPath);

        if (!File.Exists(inputPath))
        {
            File.CreateSymbolicLink(inputPath, newInputPath);
        }

        return file
            .ReplaceLineEndings()
            .Split(Environment.NewLine, options);
    }
    
    private static DayInfoAttribute? GetDayInfo(this Solution solution) =>
        solution.GetType().GetCustomAttribute<DayInfoAttribute>();
    
    private static string CallingFilePath([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;
}