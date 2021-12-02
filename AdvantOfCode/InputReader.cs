using System.Runtime.CompilerServices;

namespace AdventOfCode;

public static class InputReader
{
    public static string ReadFile([CallerFilePath] string path = null)
    {
        return File.ReadAllText(path.Replace("Solution", "input").Replace(".cs", ".txt"));
    }

    public static List<string> ReadFileLines([CallerFilePath] string path = null)
    {
        return ReadFile(path).Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}