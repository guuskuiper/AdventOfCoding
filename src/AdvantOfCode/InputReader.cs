using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace AdventOfCode;

public static class InputReader
{
    public static string ReadFile([CallerFilePath] string path = null)
    {
        return File.ReadAllText(path.Replace("Solution", "input").Replace(".cs", ".txt"));
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