using System.Reflection;
using System.Runtime.CompilerServices;

namespace AdventOfCode;

public static class DayGenerator
{
    public static Solution GetByName(string name, string middleNamespace)
    {
        var solutionType = GetSolutionTypes()
            .Where(x => MatchesNamespace(x, middleNamespace))
            .FirstOrDefault(x => x.Name == name);
        if (solutionType == null) throw new Exception($"Cannot find {middleNamespace}.{name}");
        return CreateInstance(solutionType);
    }
    
    public static SolutionAsync GetAsyncByName(string name, string middleNamespace)
    {
        var solutionType = GetSolutionAsyncTypes()
            .Where(x => MatchesNamespace(x, middleNamespace))
            .FirstOrDefault(x => x.Name == name);
        if (solutionType == null) throw new Exception($"Cannot find {middleNamespace}.{name}");
        return CreateInstance<SolutionAsync>(solutionType);
    }
    
    public static Solution GetSolutionByDay(int year, int day)
    {
        var solutionType = GetSolutionTypes()
            .FirstOrDefault(x => x.GetCustomAttribute<DayInfoAttribute>() is DayInfoAttribute dayInfo
                                 && dayInfo.Year == year 
                                 && dayInfo.Day == day);
        if (solutionType == null) throw new Exception($"Cannot find Year {year} Day {day}");
        return CreateInstance(solutionType);
    }
    
    public static IEnumerable<Solution> GetSolutionsByDay(int year, int day)
    {
        var solutionTypes = GetSolutionTypes()
            .Where(x => x.GetCustomAttribute<DayInfoAttribute>() is DayInfoAttribute dayInfo
                                 && dayInfo.Year == year 
                                 && dayInfo.Day == day);
        foreach (Type solutionType in solutionTypes)
        {
            yield return CreateInstance(solutionType);
        }
    }
    
    public static IEnumerable<SolutionAsync> GetSolutionAsyncByDay(int year, int day)
    {
        var solutionTypes = GetSolutionAsyncTypes()
            .Where(x => x.GetCustomAttribute<DayInfoAttribute>() is DayInfoAttribute dayInfo
                        && dayInfo.Year == year 
                        && dayInfo.Day == day);
        foreach (Type solutionType in solutionTypes)
        {
            yield return CreateInstance<SolutionAsync>(solutionType);
        }
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
    
    private static List<Type> GetSolutionAsyncTypes() => 
        Assembly.GetAssembly(typeof(Program))!
            .ExportedTypes
            .Where(x => x.IsAssignableTo(typeof(SolutionAsync)) && !x.IsAbstract)
            .OrderBy(x => x.Name)
            .ToList();

    private static Solution CreateInstance(Type t)
    {
        return (Activator.CreateInstance(t) as Solution)!;
    }
    
    private static T CreateInstance<T>(Type t) where T : class
    {
        return (Activator.CreateInstance(t) as T)!;
    }

    public static async Task AddDayInfoAttribute(int year)
    {
        string yearString = $"Year{year}";
        var sourcePath = GetSourceFilePathName();
        var root = Path.GetDirectoryName(sourcePath)!;
        Console.WriteLine($"Current: {root}");
        foreach (var day in Enumerable.Range(1, 25))
        {
            var dayString = $"Day{day:D2}";
            var className = $"Solution{day:D2}.cs";
            var dayPath = Path.Combine(root, yearString, dayString, className);
            
            var lines = await File.ReadAllLinesAsync(dayPath);
            var linesList = lines.ToList();
            string dayInfoText = $"[DayInfo({year}, {day:D2})]";

            int classLineIndex = linesList.FindIndex(x => x.StartsWith("public class Solution"));
            
            if (linesList[classLineIndex - 1] == dayInfoText) continue;
            
            linesList.Insert(classLineIndex, dayInfoText);
            await File.WriteAllLinesAsync(dayPath, linesList);
        }
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

            var fileName = Path.Combine(newDayPath, $"Solution{day:D2}" + ".cs");
            if(File.Exists(fileName) && !force) continue;

            string content = FillSolutionTemplate(year, day);
            await File.WriteAllTextAsync(fileName, content);
        }
    }

    public static string FillSolutionTemplate(int year, int day)
    {
        string yearString = $"Year{year}";
        var dayString = $"Day{day:D2}";
        var className = $"Solution{day:D2}";
        string content = $$"""
namespace AdventOfCode.{{yearString}}.{{dayString}};

[DayInfo({{year}}, {{day:D2}})]
public class {{className}} : SolutionAsync
{
    public Task<string> RunAsync()
    {
        string[] input = await this.ReadLinesAsync();
        return "1" + "\n";
    }
}    
""";
        return content;
    }

    static string GetSourceFilePathName([CallerFilePath] string? callerFilePath = null) //
        => callerFilePath ?? "";
}