using System.Runtime.CompilerServices;
using AdventOfCode.Client;

namespace AdventOfCode;

public static class InputReader
{
    public static string ReadFile(int year, int day, string inputFilePath)
    {
        if (!File.Exists(inputFilePath))
        {
            string text = AoCClient.DownloadAsync(year, day).Result;
            File.WriteAllText(inputFilePath, text);
            return text;
        }
        return File.ReadAllText(inputFilePath);
    }

    public static IEnumerable<string> StreamFile(string inputFilePath)
    {
        using var fs = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read);
        using TextReader sr = new StreamReader(fs);

        while (sr.ReadLine() is string line)
        {
            yield return line;
        }
    }

    public static string ReadFile([CallerFilePath] string sourceFilePath = "")
    {
        string inputPath = sourceFilePath.Replace("Solution", "input").Replace(".cs", ".txt");
        string[] split = inputPath.Split(Path.DirectorySeparatorChar);
        int year = int.Parse(split.Single(s => s.StartsWith("Year")).Substring(4));
        int day = int.Parse(split.Single(s => s.StartsWith("Day")).Substring(3));

        return ReadFile(year, day, inputPath);
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