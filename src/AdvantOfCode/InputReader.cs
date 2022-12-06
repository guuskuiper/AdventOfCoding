using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace AdventOfCode;

public static class InputReader
{
    public static string ReadFile([CallerFilePath] string path = null)
    {
        string inputPath = path.Replace("Solution", "input").Replace(".cs", ".txt");
        if (!File.Exists(inputPath))
        {
            string[] split = inputPath.Split(Path.DirectorySeparatorChar);
            int year = int.Parse(split.Single(s => s.StartsWith("Year")).Substring(4));
            int day = int.Parse(split.Single(s => s.StartsWith("Day")).Substring(3));
            string text = AoCClient.DownloadAsync(year, day).Result;
            File.WriteAllText(inputPath, text);
            return text;
        }
        return File.ReadAllText(inputPath);
    }

    private static IEnumerable<string> ReadFileLinesEnumerable(string path)
    {
        return ReadFile(path).Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);
    }

    public static List<string> ReadFileLines([CallerFilePath] string path = null)
    {
        return ReadFileLinesEnumerable(path).ToList();
    }
    
    public static string[] ReadFileLinesArray([CallerFilePath] string path = null)
    {
        return ReadFileLinesEnumerable(path).ToArray();
    }
}