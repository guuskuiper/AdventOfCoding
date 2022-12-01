using System.Reflection;
using System.Runtime.CompilerServices;

namespace AdventOfCode;

public static class DayGenerator
{
    public static Solution GetByName(string name, string middleNamespace)
    {
        var solutionType = GetSolutionTypes()
            .Where(x => MatchesNamespace(x, middleNamespace))
            .Where(x => x.Name == name)
            .FirstOrDefault();
        if (solutionType == null) throw new Exception($"Cannot find {middleNamespace}.{name}");
        return CreateInstance(solutionType);
    }

    private static bool MatchesNamespace(Type t, string name)
    {
        var split = t.Namespace?.Split('.') ;
        if (split?.Length != 3) return false;

        return split[1] == name;
    }

    private static List<Type> GetSolutionTypes() => 
        Assembly.GetAssembly(typeof(Program))!
            .ExportedTypes
            .Where(x => x.IsAssignableTo(typeof(Solution)) && !x.IsAbstract)
            .OrderBy(x => x.Name)
            .ToList();

    private static Solution CreateInstance(Type t)
    {
        return (Activator.CreateInstance(t) as Solution)!;
    }

    public static async Task CreateDirectoriesPerDay(int year, bool force = false)
    {
        string yearString = $"Year{year}";
        var sourcePath = GetSourceFilePathName();
        var root = Path.GetDirectoryName(sourcePath);
        Console.WriteLine($"Current: {root}");
        foreach (var day in Enumerable.Range(1, 25))
        {
            var dayString = $"Day{day:D2}";
            var newDayPath = Path.Combine(root, yearString, dayString);
            if (!Directory.Exists(newDayPath))
            {
                Directory.CreateDirectory(newDayPath);
            }

            var className = $"Solution{day:D2}";
            var fileName = Path.Combine(newDayPath, $"Solution{day:D2}" + ".cs");
            if(File.Exists(fileName) && !force) continue;

            string content = @""
                             + $"namespace AdventOfCode.{yearString}.{dayString};" + Environment.NewLine
                             + Environment.NewLine
                             + $"public class {className} : Solution" + Environment.NewLine
                             + "{" + Environment.NewLine
                             + "    public string Run()" + Environment.NewLine
                             + "    {" + Environment.NewLine
                             + "        return \"UNKNOWN\";" + Environment.NewLine
                             + "    }" + Environment.NewLine
                             + "}" + Environment.NewLine;
            await File.WriteAllTextAsync(fileName, content);
        }
    }

    static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) //
        => callerFilePath ?? "";
}