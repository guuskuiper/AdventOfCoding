using System.Drawing;
using AdventOfCode.Graph;

namespace AdventOfCode.Extensions;

public static class ArrayExtensions
{
    public static IEnumerable<T> ToEnumerable<T>(this T[,] target)
    {
        foreach (var item in target)
            yield return item;
    }
    
    public static IEnumerable<T> ToEnumerable<T>(this T[,,] target)
    {
        foreach (var item in target)
            yield return item;
    }

    public static char[,] ToGrid(this string[] lines) => ToGrid(lines, Identity);

    public static RectValueGrid<char> ToRecGrid(this string[] lines, Size[]? neightbors = null) => ToRecGrid<char>(lines, Identity, neightbors);
    public static RectValueGrid<T> ToRecGrid<T>(this string[] lines, Func<char, T> parse, Size[]? neightbors = null) => new(lines.ToGrid(parse), neightbors);
    private static char Identity(char c) => c;
    
    public static T[,] ToGrid<T>(this string[] lines, Func<char, T> parse)
    {
        int width = lines[0].Length;
        int height = lines.Length;

        T[,] grid = new T[width, height];
        for (int y = 0; y < height; y++)
        {
            var line = lines[y];
            for (int x = 0; x < width; x++)
            {
                char c = line[x];
                T v = parse(c);
                grid[x,y] = v;
            }
        }

        return grid;
    }

    public static int Width<T>(this T[,] array) => array.GetLength(0);
    public static int Heigth<T>(this T[,] array) => array.GetLength(1);
}