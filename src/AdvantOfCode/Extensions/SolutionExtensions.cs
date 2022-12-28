using System.Reflection;
using System.Runtime.CompilerServices;

namespace AdventOfCode.Extensions;

public static class SolutionExtensions
{
    public static DayInfoAttribute? GetDayInfo(this Solution solution) =>
        solution.GetType().GetCustomAttribute<DayInfoAttribute>();
    
    public static string[] ReadLines(this Solution solution, [CallerFilePath] string path = "")
    {
        string inputPath = path.Replace("Solution", "input").Replace(".cs", ".txt");
        DayInfoAttribute? dayInfo = GetDayInfo(solution);

        string file = dayInfo is not null ? 
            InputReader.ReadFile(dayInfo.Year, dayInfo.Day, inputPath) :
            InputReader.ReadFile(path);
        return file.Split('\n');
    }
}